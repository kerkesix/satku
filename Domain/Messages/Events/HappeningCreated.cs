namespace KsxEventTracker.Domain.Messages.Events
{
    public class HappeningCreated : EventBase, IEvent
    {
        public string HappeningId { get; set; }

        public HappeningCreated()
        {
        }

        public HappeningCreated(string id)
        {
            this.HappeningId = id;
        }
    }
}