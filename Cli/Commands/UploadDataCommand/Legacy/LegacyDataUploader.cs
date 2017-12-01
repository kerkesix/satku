namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Repositories;

    public class LegacyDataUploader : IUploader
    {
        public Task Upload(string environment)
        {
            Console.Out.WriteLine("Uploading legacy data to " + environment);

            var env = new EnvironmentClient(environment);

            Console.Out.WriteLine("Using connection string " + env.Options.ConnectionString);

            var eventStore = new EventStream(env.Options);
            var converter = new DatabaseDumpToEventsInitializer();
            var appendTasks = converter.LegacyDataAsEvents()
                .Select(namedEventStream => 
                    eventStore.AppendEvents(namedEventStream.Name, namedEventStream.Events))
                .ToList();

            return Task.WhenAll(appendTasks);
        } 
    }
}