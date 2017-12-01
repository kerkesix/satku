namespace KsxEventTracker.Domain.Repositories
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class AzureTableStorageRepositoryBase
    {
        protected CloudTable Table;

        public AzureTableStorageRepositoryBase(string tableName, AzureTableStorageOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            this.Table = tableClient.GetTableReference(tableName);
            this.Table.CreateIfNotExistsAsync().Wait();
        }
    }
    
    public class AzureTableStorageOptions {
        public string ConnectionString { get; set; }
    }
}