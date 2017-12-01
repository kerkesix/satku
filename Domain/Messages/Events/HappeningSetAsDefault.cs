namespace KsxEventTracker.Domain.Messages.Events
{
    public class HappeningSetAsDefault : EventBase, IEvent
    {
        public HappeningSetAsDefault(string id, string keyOfNewDefault): base(id)
        {
            this.KeyOfNewDefault = keyOfNewDefault;
        }

        public string KeyOfNewDefault { get; private set; }
    }
}