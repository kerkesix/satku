namespace KsxEventTracker.Domain.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Messages.Events;

    using Microsoft.WindowsAzure.Storage.Table;

    public class EventStream : AzureTableStorageRepositoryBase
    {
        public EventStream(AzureTableStorageOptions options): base("events", options)
        {   
        }

        public async Task<IEnumerable<EventEntity>> AppendEvents(string streamName, List<IEvent> thenEvents)
        {
            IEnumerable<EventEntity> eventEntities = thenEvents != null ?             
                thenEvents.Select(m => new EventEntity(streamName, m.Id, m.Timestamp, m)) 
                : new List<EventEntity>();
            
            // TODO: Do something with the results (what if some of them failed?)
            await Add(eventEntities);
            return eventEntities;
        }

        public async Task<IEnumerable<IEvent>> ReadAllEvents()
        {
            var query = new TableQuery<EventEntity>();
            return await this.ReadAllEventsCore(query);
        }

        public async Task<IEnumerable<IEvent>> ReadAllEvents(string streamName)
        {
            var query = new TableQuery<EventEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, streamName));
            return await this.ReadAllEventsCore(query);
        }

        private async Task<IEnumerable<IEvent>> ReadAllEventsCore(TableQuery<EventEntity> query)
        {
            TableQuerySegment<EventEntity> querySegment = null;
            var returnList = new List<EventEntity>();
            
            while(querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await Table.ExecuteQuerySegmentedAsync(
                    query, querySegment != null ? querySegment.ContinuationToken : null);
                returnList.AddRange(querySegment);
            }
            
            return returnList.OrderBy(m => m.EventTimestamp).Select(m => (IEvent)m.DeSerializeEvent());   
        }

        /// <summary>
        /// Gets a single event from the event store.
        /// </summary>
        /// <param name="streamName">Name of the stream (partition key)</param>
        /// <param name="id">Id of the event (row key).</param>
        /// <returns>Event, or null if not found.</returns>
        public IEvent GetEvent(string streamName, string id)
        {
            var eventEntityAsync = this.GetEventEntity(streamName, id);
            Task.WaitAll(eventEntityAsync);
            var eventEntity = eventEntityAsync.Result;

            if (eventEntity != null)
            {
                // Caller knows the type of this event, we do not
                return (IEvent)eventEntity.DeSerializeEvent();
            }

            return null;
        }

        /// <summary>
        /// Gets a single event from the event store.
        /// </summary>
        /// <param name="streamName">Name of the stream (partition key)</param>
        /// <param name="id">Id of the event (row key).</param>
        /// <returns>Event, or null if not found.</returns>
        public T GetEvent<T>(string streamName, string id) where T: IEvent
        {
            var eventEntityAsync = this.GetEventEntity(streamName, id);
            Task.WaitAll(eventEntityAsync);
            var eventEntity = eventEntityAsync.Result;

            return eventEntity != null ? eventEntity.DeSerializeEvent<T>() : default(T);
        }

        private async Task<EventEntity> GetEventEntity(string streamName, string id)
        {
            var operation = TableOperation.Retrieve<EventEntity>(streamName, id);
            var result = await this.Table.ExecuteAsync(operation);

            EventEntity eventEntity = null;

            if (result.Result != null)
            {
                eventEntity = (EventEntity)result.Result;
            }
            return eventEntity;
        }

        /// <summary>
        /// Inserts a batch of events into database.
        /// </summary>
        /// <param name="eventEntities">Events to add.</param>
        /// <returns>Operation result.</returns>
        private async Task<IList<TableResult>> Add(IEnumerable<EventEntity> eventEntities)
        {
            var batch = new TableBatchOperation();
            
            foreach (var tableOperation in eventEntities.Select(TableOperation.Insert))
            {
                batch.Add(tableOperation);           
            }

            return await this.Table.ExecuteBatchAsync(batch);
        }
    }
}