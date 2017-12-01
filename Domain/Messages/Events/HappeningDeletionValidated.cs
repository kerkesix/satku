namespace KsxEventTracker.Domain.Messages.Events
{
    public class HappeningDeletionValidated : EventBase, IEvent
    {
        public HappeningDeletionValidated(string happeningId)
        {
            this.HappeningId = happeningId;
        }

        public string HappeningId { get; set; }
    }
}