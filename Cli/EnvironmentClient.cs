namespace KsxEventTracker.Cli
{
    using KsxEventTracker.Domain.Repositories;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Microsoft.Extensions.Configuration;

    internal class EnvironmentClient
    {
        private const string ConnectionStringPrefix = "StorageConnectionStrings";

        internal const string ProductionSuffix = "Production";

        private readonly CloudTableClient tableClient;

        public string TargetEnvironment { get; set; }

        public EnvironmentClient(string targetEnvironment)
        {
            this.TargetEnvironment = targetEnvironment;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.Options.ConnectionString);
            this.tableClient = storageAccount.CreateCloudTableClient();
        }

        public AzureTableStorageOptions Options
        {
            get
            {
                return new AzureTableStorageOptions {
                    ConnectionString = Program.Configuration.GetValue<string>($"{ConnectionStringPrefix}:{TargetEnvironment}")
                };
            }
        }

        public CloudTable GetTable(string name)
        {
            return this.tableClient.GetTableReference(name);
        }
    }
}