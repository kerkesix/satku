namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    public class EventBase : IEvent
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public EventBase()
        {
            this.Timestamp = DateTimeOffset.UtcNow;
            this.Id = Guid.NewGuid().ToString();
        }

        public EventBase(string id): this(id, DateTimeOffset.UtcNow)
        {
        }

        public EventBase(string id, DateTimeOffset timestamp)
        {
            this.Id = id;
            this.Timestamp = timestamp;
        }
    }
}