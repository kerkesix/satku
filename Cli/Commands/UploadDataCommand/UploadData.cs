namespace KsxEventTracker.Cli.Commands.UploadDataCommand
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Legacy;
    using satkuxii;

    public class UploadData : ICliCommandHandler
    {
        private readonly Dictionary<string, IUploader> uploaders = new Dictionary<string, IUploader>
                                                          {
                                                              { "legacy", new LegacyDataUploader()},
                                                              { "satkuxii", new SatkuxiiUploader() }
                                                          };

        public string Usage => "targetEnvironment datasetName";
        public string Help => "Uploads data to the given target environment. " + AvailableDataSetNames;
        private string AvailableDataSetNames => "Available data set names: " + string.Join(", ", uploaders.Keys) + ".";
           
        public Task Execute(params string[] args)
        {
            string target;

            if (!ArgsHelper.GetTargetEnvironmentFromArgs(args, out target, true))
            {
                return Task.FromResult(false);
            }

            if (args.Length < 2)
            {
                Console.Out.WriteLine("Must give dataset name as second argument to UploadData. " + AvailableDataSetNames);
                return Task.FromResult(false);
            }

            string datasetName = args[1];

            if (!this.uploaders.ContainsKey(datasetName))
            {
                Console.Out.WriteLine("Invalid dataset name " + datasetName + ". " + AvailableDataSetNames);
                return Task.FromResult(false);
            }

            Console.Out.WriteLine("Uploading data {0} to environment {1}.", datasetName, target);

            return this.uploaders[datasetName].Upload(target);
        }
    }
}