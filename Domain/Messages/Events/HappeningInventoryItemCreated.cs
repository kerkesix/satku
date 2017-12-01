namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    public class HappeningInventoryItemCreated : EventBase, IEvent
    {
        public HappeningInventoryItemCreated()
        {
        }

        public HappeningInventoryItemCreated(string id, bool isDefault)
            : this(id, isDefault, DateTime.UtcNow)
        {
        }

        public HappeningInventoryItemCreated(string id, bool isDefault, DateTimeOffset timestamp)
            : base(id, timestamp)
        {
            this.IsDefault = isDefault;
        }

        public bool IsDefault { get; set; }
    }
}