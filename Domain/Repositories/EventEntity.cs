namespace KsxEventTracker.Domain.Repositories
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    public class EventEntity: TableEntity
    {
        public EventEntity()
        {
        }

        public EventEntity(string partitionKey, string rowKey, DateTimeOffset timestamp, object eventToSerialize)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;

            // Cannot use system provided Timestamp property, create own.
            this.EventTimestamp = timestamp;
            this.Data = JsonConvert.SerializeObject(eventToSerialize);
            this.DataType = eventToSerialize.GetType().FullName;
        }

        /// <summary>
        /// Business timestamp of the event (opposed to Table storage internal 
        /// Timestamp usage which cannot be controlled).
        /// </summary>
        public DateTimeOffset EventTimestamp { get; set; }

        /// <summary>
        /// Event data serialized as JSON.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Original event's data type.
        /// </summary>
        public string DataType { get; set; }

        public object DeSerializeEvent()
        {
            var t = Type.GetType(this.DataType);
            return JsonConvert.DeserializeObject(this.Data, t);
        }

        public T DeSerializeEvent<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Data);
        }
    }
}