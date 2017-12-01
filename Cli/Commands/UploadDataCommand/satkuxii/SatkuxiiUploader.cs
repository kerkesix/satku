namespace KsxEventTracker.Cli.Commands.UploadDataCommand.satkuxii
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using KsxEventTracker.Domain.Repositories;

    using Newtonsoft.Json;

    internal class SatkuxiiUploader: IUploader
    {
        public Task Upload(string environment)
        {
            Console.Out.WriteLine("Uploading satkuxii data to " + environment);

            var env = new EnvironmentClient(environment);

            Console.Out.WriteLine("Using connection string " + env.Options.ConnectionString);

            var jsonData = GetEmbeddedResource("satkuxii.json");
            var data = JsonConvert.DeserializeObject<SatkuData>(jsonData);

            Console.Out.WriteLine("Deserialized, checkpoint count " + data.Checkpoints.Count );

            var stream = new EventStream(env.Options);

            stream.AppendEvents(
                "HappeningsInventory-" + HappeningMessageHandler.RootAggregateId,
                new List<IEvent>
                {
                    new HappeningInventoryItemCreated(
                        data.Happening,
                        data.IsDefault,
                        data.Checkpoints.First().Timestamp.AddSeconds(-10))
                }).Wait();

            var happeningCreated = new HappeningCreated(data.Happening)
                                   {
                                       Timestamp =
                                           data.Checkpoints.First()
                                           .Timestamp.AddSeconds(-9)
                                   };
            stream.AppendEvents(
                "Happening-" + data.Happening,
                new List<IEvent> { happeningCreated }).Wait();


            var checkpointListEvents = new List<IEvent>();
            int order = 1;

            foreach (var c in data.Checkpoints)
            {
                // To keep data simpler to edit by hand, set some properties automatically
                c.HappeningId = data.Happening;
                c.Order = order;
                order ++;

                var validatedEvent = new CheckpointValidated(
                                        data.Happening,
                                        c.Id,
                                        c.Order,
                                        c.CheckpointType,
                                        c.Name,
                                        c.Latitude,
                                        c.Longitude,
                                        c.DistanceFromPrevious,
                                        c.DistanceFromStart)
                {
                    // Use timestamp little after the happening
                    Timestamp = c.Timestamp.AddSeconds(-1)
                };

                checkpointListEvents.Add(validatedEvent);
                stream.AppendEvents("Checkpoint-" + c.Id, new List<IEvent> { c }).Wait();
            }

            return stream.AppendEvents("CheckpointList-" + data.Happening, checkpointListEvents);
        }

        private static string GetEmbeddedResource(string resourceName)
        {
            const string ResourceNamespace = "KsxEventTracker.Cli.Commands.UploadDataCommand.satkuxii";

            using (
                var stream = typeof(SatkuxiiUploader).GetTypeInfo().Assembly.GetManifestResourceStream(ResourceNamespace + "." + resourceName))
            {
                if (stream == null)
                {
                    throw new Exception("Resource " + resourceName + " not found");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    internal class SatkuData
    {
        public string Happening { get; set; }
        public bool IsDefault { get; set; }
        public List<CheckpointCreated> Checkpoints { get; private set; }

        public SatkuData()
        {
            this.Checkpoints = new List<CheckpointCreated>();
        }
    }
}
