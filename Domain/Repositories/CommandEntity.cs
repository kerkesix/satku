namespace KsxEventTracker.Domain.Repositories
{
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    public class CommandEntity : TableEntity
    {
        public string UserName { get; set; }

        /// <summary>
        /// Event data serialized as JSON.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Original event's data type.
        /// </summary>
        public string DataType { get; set; }

        public CommandEntity()
        {
        }

        public CommandEntity(string partitionKey, string rowKey, string userName, object commandToSerialize)
        {
            this.UserName = userName;
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            this.Data = JsonConvert.SerializeObject(commandToSerialize).ToGzip();
            this.DataType = commandToSerialize.GetType().FullName;
        }
    }
}