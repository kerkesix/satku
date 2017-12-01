namespace Web.QueryModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain;
    using KsxEventTracker.Domain.Aggregates.Checkpoint;
    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;
    using Web.Logic;
    using Web.Logic.Destruction;

    public class DashboardDenormalizer : IDenormalizer
    {
        readonly AttendeeSpeedCalculator calculator = new AttendeeSpeedCalculator();
        private readonly ILoggerFactory loggerFactory;

        public DashboardDenormalizer(ILoggerFactory loggerFactory) {
            this.loggerFactory = loggerFactory;
        }

        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        #region Happening
        public void When(HappeningCreated e)
        {
            QueryModelRepository.Dashboard.Happenings.Add(e.HappeningId, new HappeningDashboard());
        }

        public void When(HappeningDeleted e)
        {
            QueryModelRepository.Dashboard.Happenings.Remove(e.HappeningId);
        }

        public void When(CoordinatePathSet e)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[e.HappeningId];
            happening.Path = e.Path.FromGzip();
        }

        #endregion

        #region Checkpoint
        public void When(CheckpointCreated e)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[e.HappeningId];

            if (happening != null)
            {
                happening.AddCheckpoint(new DashboardCheckpoint
                                          {
                                              Id = e.Id,
                                              Name = e.Name,
                                              DistanceFromPrevious = e.DistanceFromPrevious,
                                              DistanceFromStart = e.DistanceFromStart,
                                              Latitude = e.Latitude,
                                              Longitude = e.Longitude,
                                              Order = e.Order,
                                              CheckpointType = e.CheckpointType
                                          });
            }
        }

        public void When(CheckpointDeleted e)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[e.HappeningId];

            if (happening != null)
            {
                happening.Checkpoints.RemoveAll(
                    m => m.Id.Equals(e.Id, StringComparison.OrdinalIgnoreCase));
            }
        } 
        #endregion

        #region Attendee scans
        public void When(AttendeeScanIn e)
        {
            var checkpoint = this.ApplyScan(e, e.ScanTimestamp, null);

            // Remove expected at from this checkpoint
            checkpoint.Expected.Remove(e.PersonId);
        }

        public void When(AttendeeScanOut e)
        {
            var checkpoint = this.ApplyScan(e, null, e.ScanTimestamp);

            // Add expected at next checkpoint
            this.AddExpectedAt(e.HappeningId, e.ScanTimestamp, e.PersonId, checkpoint);
        }

        public void When(AttendeePassthroughScan e)
        {
            var checkpoint = this.ApplyScan(e, e.ScanTimestamp, e.ScanTimestamp);

            // Add expected at next checkpoint
            this.AddExpectedAt(e.HappeningId, e.ScanTimestamp, e.PersonId, checkpoint);

            // Remove expected at from this checkpoint
            checkpoint.Expected.Remove(e.PersonId);
        }

        public void When(AttendeeScanInTimeChanged e)
        {
            this.ApplyScan(e, e.NewTime, null);
        }

        public void When(AttendeeScanOutTimeChanged e)
        {
            this.ApplyScan(e, null, e.NewTime);
        }

        public void When(AttendeePassthroughScanTimeChanged e)
        {
            this.ApplyScan(e, e.NewTime, e.NewTime);
        }

        public void When(AttendeeScanInRemoved e)
        {
            var checkpoint = GetCheckpoint(e.HappeningId, e.CheckpointId);

            if (checkpoint == null)
            {
                return;
            }

            checkpoint.Visits.RemoveAll(
                v => v.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            // Re-add expected at-date
            var happening = QueryModelRepository.Dashboard.Happenings[e.HappeningId];
            var previous = GetPreviousCheckpoint(happening.Checkpoints, checkpoint);

            if (previous == null)
            {
                return;
            }

            var visit = previous.Visits.FirstOrDefault(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            if (visit != null && visit.TimeOut.HasValue)
            {
                this.AddExpectedAt(e.HappeningId, visit.TimeOut.Value, e.PersonId, previous);
            }
        }

        public void When(AttendeeScanOutRemoved e)
        {
            var checkpoint = GetCheckpoint(e.HappeningId, e.CheckpointId);
            var visit = checkpoint?.Visits.FirstOrDefault(
               m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            if (visit == null)
            {
                return;
            }

            if (visit.TimeIn.HasValue)
            {
                visit.TimeOut = null;
            }
            else
            {
                // This is the out scan from beginning
                checkpoint.Visits.Remove(visit);
            }

            // Remove expected at from next checkpoint
            RemoveExpected(e.PersonId, e.HappeningId, checkpoint);
        }

        public void When(AttendeeQuit e)
        {
            var checkpoint = GetCheckpoint(e.HappeningId, e.CheckpointId);
            var personVisit = checkpoint?.Visits.FirstOrDefault(MatchVisit(e.PersonId));

            if (personVisit == null)
            {
                return;
            }
            
            personVisit.Quit = new DashboardQuit
                               {
                                   Timestamp = e.ScanTimestamp,
                                   WalkedSinceLastCheckpoint = e.WalkedSinceLast,

                                   // TODO: This should be filtered out for unauthenticated users
                                   Description = e.Description
                               };

            // Remove expected at from next checkpoint
            RemoveExpected(e.PersonId, e.HappeningId, checkpoint);

            var person = QueryModelRepository.Dashboard.People.First(MatchPerson(e.PersonId));
            person.QuitLastTime = true;
            person.HappeningsQuit.Add(e.HappeningId);

            CalculateDestructionPercent(e.HappeningId, e.PersonId);
        }

        public void When(AttendeeCompleted e)
        {
            var person = QueryModelRepository.Dashboard.People.First(MatchPerson(e.PersonId));
            person.QuitLastTime = false;
            person.HappeningsCompleted.Add(e.HappeningId);
            CalculateDestructionPercent(e.HappeningId, e.PersonId);
        }

        #endregion

        public void When(PersonCreated e)
        {
            QueryModelRepository.Dashboard.People.HandleCreate(e);
            SortAttendeesByDisplayName();
        }

        public void When(PersonContactInformationUpdated e)
        {
            QueryModelRepository.Dashboard.People.HandleUpdate(e);
        }

        public void When(PersonLinkedToHappening e)
        {
            QueryModelRepository.Dashboard.People.HandleLinkedToHappening(e);

            var person =
                QueryModelRepository.Dashboard.People.First(MatchPerson(e.PersonId));

            // Add information to happening
            QueryModelRepository.Dashboard.Happenings[e.HappeningId].Attendees.Add(
                new DashboardAttendee
                {
                    Person = person
                }
            );

            SortAttendeesByDisplayName();
            CalculateDestructionPercent(e.HappeningId, e.PersonId);
        }

        public void When(PersonUnlinkedFromHappening e)
        {
            QueryModelRepository.Dashboard.People.HandleUnlinkedFromHappening(e);

            // Change happening information also
            QueryModelRepository.Dashboard.Happenings[e.HappeningId].Attendees.RemoveAll(
                m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
        }

        public void When(PersonDeleted e)
        {
            QueryModelRepository.Dashboard.People.HandleDelete(e);

            // As delete is never called if person is linked to any happenings --> no need to clean up anything
        }

        #region Person

        #endregion

        private static void AddOrUpdateVisit(
            List<DashboardVisit> visits, 
            string personId, 
            DateTimeOffset? timeIn, 
            DateTimeOffset? timeOut)
        {
            var visit = visits.FirstOrDefault(
                m => m.PersonId.Equals(personId, StringComparison.OrdinalIgnoreCase));

            if (visit == null)
            {
                visits.Add(new DashboardVisit
                           {
                               PersonId = personId,
                               TimeIn = timeIn,
                               TimeOut = timeOut
                           });
            }
            else
            {
                visit.TimeIn = timeIn ?? visit.TimeIn;
                visit.TimeOut = timeOut ?? visit.TimeOut;
            }
        }

        private static DashboardCheckpoint GetCheckpoint(string happeningId, string checkpointId)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[happeningId];
            return happening == null ? null : happening.Checkpoints.FirstOrDefault(c => c.Id.Equals(checkpointId, StringComparison.OrdinalIgnoreCase));
        }

        private static DashboardCheckpoint GetPreviousCheckpoint(
            IList<DashboardCheckpoint> checkpoints, DashboardCheckpoint checkpoint)
        {
            int prevIndex = checkpoints.IndexOf(checkpoint) - 1;
            return prevIndex > 0 ? checkpoints[prevIndex] : null;
        }

        private static DashboardCheckpoint GetNextCheckpoint(
            IList<DashboardCheckpoint> checkpoints, DashboardCheckpoint checkpoint)
        {
            int nextIndex = checkpoints.IndexOf(checkpoint) + 1;
            return checkpoints.Count() > nextIndex ? checkpoints[nextIndex] : null;
        }
        private static void SortAttendeesByDisplayName()
        {
            foreach (var h in QueryModelRepository.Dashboard.Happenings.Values)
            {
                h.Attendees.Sort(
                    (x, y) => String.Compare(x.Person.DisplayName, y.Person.DisplayName, StringComparison.OrdinalIgnoreCase));
            }
        }

        private void CalculateDestructionPercent(string happeningId, string personId)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[happeningId];
            var attendee = happening.Attendees.First(MatchAttendee(personId));
            var person = attendee.Person;

            if (person.HappeningsQuit.Contains(happeningId))
            {
                attendee.DestructionPercent = 100;
                return;
            }

            if (person.HappeningsCompleted.Contains(happeningId))
            {
                attendee.DestructionPercent = 0;
                return;
            }

            var currentDistance =
                happening.Checkpoints.SelectMany(
                    m => m.Visits.Where(MatchVisit(personId)),
                    (checkpoint, visit) => checkpoint.DistanceFromStart).DefaultIfEmpty(0).Max();


            var speed = this.calculator.Calculate(happening.Checkpoints.OrderBy(a => 1), personId);
            
            var destructionCalculator =
                CalculatorFactory.Create(
                    new PersonData
                        {
                            DestructedInPrevious = person.QuitLastTime,
                            Attended = person.HappeningsAttended.Count,
                            Destructed = person.HappeningsQuit.Count
                        });

            attendee.DestructionPercent =
                destructionCalculator.Calculate(
                    new WalkingData
                        {
                            LastAverageSpeedChange = (decimal)speed.LastAverageSpeedChangePercent,
                            CurrentDistance = currentDistance
                        });
        }

        #region Matching functions
        private static Func<Person, bool> MatchPerson(string personId)
        {
            return Match(personId);
        }

        private static Func<DashboardAttendee, bool> MatchAttendee(string personId)
        {
            return Match(personId);
        }

        private static Func<DashboardVisit, bool> MatchVisit(string personId)
        {
            return Match(personId);
        }

        private static Func<IPersonOwned, bool> Match(string personId)
        {
            return p => p.PersonId.Equals(personId, StringComparison.OrdinalIgnoreCase);
        } 
        #endregion

        private void AddExpectedAt(string happeningId, DateTimeOffset lastExit, string personId, DashboardCheckpoint currentCheckpoint)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[happeningId];
            var next = GetNextCheckpoint(happening.Checkpoints, currentCheckpoint);

            if (next == null || next.CheckpointType == CheckpointType.Start)
            {
                return;
            }

            var speed = this.calculator.Calculate(happening.Checkpoints.OrderBy(a => 1), personId);

            // Calculate speed to estimate expected at
            var expectedTime =
                lastExit.Add(TimeSpan.FromHours((double)next.DistanceFromPrevious / speed.WeightedSpeed));
            next.AddExpected(personId, expectedTime);
        }

        private static void RemoveExpected(
            string personId,
            string happeningId,
            DashboardCheckpoint checkpoint)
        {
            var happening = QueryModelRepository.Dashboard.Happenings[happeningId];
            var next = GetNextCheckpoint(happening.Checkpoints, checkpoint);

            if (next != null)
            {
                next.Expected.Remove(personId);
            }
        }

        private DashboardCheckpoint ApplyScan(PersonAtCheckpointBase e, DateTimeOffset? timeIn, DateTimeOffset? timeOut)
        {
            var checkpoint = GetCheckpoint(e.HappeningId, e.CheckpointId);

            if (checkpoint == null)
            {
                return null;
            }

            AddOrUpdateVisit(checkpoint.Visits, e.PersonId, timeIn, timeOut);
            this.CalculateDestructionPercent(e.HappeningId, e.PersonId);
            return checkpoint;
        }
    }
}