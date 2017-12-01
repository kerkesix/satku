namespace KsxEventTracker.Domain.Messages.Events
{
    public class CheckpointDeleted: EventBase, IEvent
    {
        public CheckpointDeleted(string happeningId, string id): base(id)
        {
            this.HappeningId = happeningId;
        }

        public string HappeningId { get; set; }
    }
}