namespace KsxEventTracker.Domain.Messages.Events
{
    public class HappeningDeleted : EventBase, IEvent
    {
        public string HappeningId { get; set; }

        public HappeningDeleted()
        {
        }

        public HappeningDeleted(string id)
        {
            this.HappeningId = id;
        }
    }
}