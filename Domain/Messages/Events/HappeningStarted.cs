namespace KsxEventTracker.Domain.Messages.Events
{
    public class HappeningStarted : EventBase, IEvent
    {
        public string HappeningId { get; private set; }

        public HappeningStarted(string happeningId)
        {
            this.HappeningId = happeningId;
        }
    }
}