namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;
    using KsxEventTracker.Domain;
    using KsxEventTracker.Domain.Aggregates.Checkpoint;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;

    public class DatabaseDumpToEventsInitializer
    {
        private readonly DumpedDataParser parser = new DumpedDataParser();

        public IEnumerable<NamedEventStream> LegacyDataAsEvents()
        {
            var happeningInventoryStream =
                new NamedEventStream("HappeningsInventory-" + HappeningMessageHandler.RootAggregateId);
            var happeningStreams = new List<NamedEventStream>();
            var checkpointStreams = new List<NamedEventStream>();
            var checkpoints = this.parser.Checkpoints;
            var happeningsCreated = new Dictionary<string, DateTime>();

            foreach (var h in this.parser.Happenings)
            {
                happeningsCreated.Add(h.Id, h.CreatedAt);
                happeningInventoryStream.Events.Add(new HappeningInventoryItemCreated(h.Id, h.Default, h.CreatedAt.ToUtcOffset()));

                var happeningStream = new NamedEventStream(CreateHappeningStreamId(h.Id));
                happeningStreams.Add(happeningStream);

                var happeningCreated = new HappeningCreated(h.Id) { Timestamp = h.CreatedAt.ToUtcOffset() };
                happeningStream.Events.Add(happeningCreated);

                var checkpointListStream = new NamedEventStream("CheckpointList-" + h.Id);
                checkpointStreams.Add(checkpointListStream);

                var h1 = h;
                foreach (var c in checkpoints.Where(m => m.HappeningId == h1.Id))
                {
                    var validatedEvent = new CheckpointValidated(
                        h.Id, 
                        c.Id.ToString(), 
                        c.Order, 
                        c.CheckpointType, 
                        c.Name, 
                        c.Location.Latitude, 
                        c.Location.Longitude,
                        c.DistanceFromPreviousCheckpoint, 
                        c.DistanceFromStart)
                                         {
                                             // Use timestamp little after the happening
                                             Timestamp = h.CreatedAt.AddSeconds(1).ToUtcOffset()
                                         };

                    checkpointListStream.Events.Add(validatedEvent);

                    var checkpointStream = new NamedEventStream(CreateCheckpointStreamId(c.Id));
                    checkpointStreams.Add(checkpointStream);

                    var createdEvent = new CheckpointCreated(
                        h.Id, 
                        c.Id.ToString(), 
                        c.CheckpointType, 
                        c.Order, 
                        c.Name, 
                        c.Location.Latitude, 
                        c.Location.Longitude,
                        c.DistanceFromPreviousCheckpoint, 
                        c.DistanceFromStart)
                                       {
                                           // Use a timestamp that is little after validated event
                                           Timestamp = validatedEvent.Timestamp.AddSeconds(1)
                                       };

                    checkpointStream.Events.Add(createdEvent);
                }
            }

            yield return happeningInventoryStream;

            int anonymousWalkers = 1;
            var attendees = this.parser.Attendees.ToList();

            var personIdMap = new Dictionary<Guid, string>();

            foreach (var p in this.parser.People)
            {
                var personStream = new NamedEventStream("Person-" + p.Id);
                string displayName = p.Name;

                if (!p.MayShowNameToPublic)
                {
                    displayName = string.Format(
                        CultureInfo.CurrentCulture, "Anonyymi kävelijä {0:n0}", anonymousWalkers);
                    anonymousWalkers++;
                }

                var namesplit = p.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var lastname = namesplit[0];
                var firstname = string.Join(" ", namesplit.Skip(1));
                personIdMap.Add(p.Id, p.NewEventBasedId);

                var personCreatedEvent = new PersonCreated(
                    p.NewEventBasedId,
                    null,
                    lastname,
                    firstname,
                    displayName,
                    new ContactInformation(p.Phone, p.Email,p.TwitterName),
                    null)
                                         {
                                             // Create all legacy persons immediately after first happening was created,
                                             // as otherwise they would not been available on AttendeeScanIn etc. events.
                                             Timestamp = happeningsCreated.Values.Min().AddSeconds(1).ToUtcOffset()
                                         };

                personStream.Events.Add(personCreatedEvent);

                // Get attendance info for this person
                personStream.Events.AddRange(
                    attendees.Where(m => m.PersonId == p.Id)
                        .Select(a =>
                                {
                                    var linkEvent = new PersonLinkedToHappening(
                                        a.Id.ToString(),
                                        p.NewEventBasedId,
                                        a.HappeningId, 
                                        null)
                                                    {
                                                        // Link immediately after happening has been created
                                                        Timestamp = happeningsCreated[a.HappeningId].AddSeconds(2).ToUtcOffset()
                                                    };
                                    return linkEvent;
                                }));

                yield return personStream;
            }

            var startedEmittedTo = new HashSet<string>();

            foreach (var parsedReading in this.parser.Readings)
            {
                var reading = parsedReading.Entity;
                var readingCheckpoint = checkpoints.First(m => m.Id == reading.CheckpointId);
                var scanInfo = new ScanInfo(
                    readingCheckpoint.HappeningId,
                    reading.CheckpointId.ToString(),
                    personIdMap[attendees.First(m => m.Id == parsedReading.AttendeeId).PersonId],
                    "fooid",
                    reading.Timestamp.ToUtcOffset(),
                    reading.Timestamp.ToUtcOffset(),
                    reading.SavedBy);

                var readingEvent = reading.ReadingType == ReadingType.In
                    ? (AttendeeScanBase)new AttendeeScanIn(scanInfo)
                    : new AttendeeScanOut(scanInfo);

                readingEvent.Id = reading.Id.ToString();
                readingEvent.ScanId = readingEvent.Id;

                // Find the correct checkpoint stream to append this reading event
                var cpStream = checkpointStreams.First(m => m.Name.Equals(CreateCheckpointStreamId(reading.CheckpointId)));
                cpStream.Events.Add(readingEvent);

                if (readingCheckpoint.CheckpointType == CheckpointType.Start 
                    && startedEmittedTo.Add(readingCheckpoint.HappeningId))
                {
                    // Emit happening started event
                    var happeningStartedEvent = new HappeningStarted(readingCheckpoint.HappeningId)
                                                {
                                                    Timestamp = readingEvent.Timestamp.AddSeconds(1)
                                                };
                    cpStream.Events.Add(happeningStartedEvent);
                }

                if (readingCheckpoint.CheckpointType == CheckpointType.Finish)
                {
                    // Emit attendee completed event
                    var completedEvent = new AttendeeCompleted(
                        readingEvent.HappeningId,
                        readingEvent.PersonId) { Timestamp = readingEvent.Timestamp };
                    cpStream.Events.Add(completedEvent);
                }
            }

            foreach (var parsedQuitter in this.parser.Quitters)
            {
                var quit = parsedQuitter.Entity;
                
                var scanInfo = new ScanInfo(
                    checkpoints.First(m => m.Id == quit.CheckpointId).HappeningId,
                    quit.CheckpointId.ToString(),
                    personIdMap[attendees.First(m => m.Id == parsedQuitter.AttendeeId).PersonId],
                    quit.Id.ToString(),
                    quit.Timestamp.ToUtcOffset(),
                    quit.Timestamp.ToUtcOffset(),
                    quit.SavedBy);

                var quitEvent = new AttendeeQuit(
                    scanInfo,
                    quit.WalkedSinceLastCheckpoint,
                    quit.Description)
                                {
                                    // Use the same ID as original data
                                    Id = quit.Id.ToString()
                                };

                // Find the correct stream to append this quit event
                var s = happeningStreams.First(m => m.Name.Equals(CreateHappeningStreamId(scanInfo.HappeningId)));
                s.Events.Add(quitEvent);
            }

            // Individual happening streams
            foreach (var s in happeningStreams)
            {
                yield return s;
            }

            foreach (var s in checkpointStreams)
            {
                yield return s;
            }
        }

        private static string CreateCheckpointStreamId(Guid id)
        {
            return "Checkpoint-" + id;
        }

        private static string CreateHappeningStreamId(string id)
        {
            return "Happening-" + id;
        }

        public class NamedEventStream
        {
            public NamedEventStream(string name)
            {
                this.Name = name;
                this.Events = new List<IEvent>();
            }
            public string Name { get; private set; }

            public List<IEvent> Events { get; private set; }
        }
    }
}