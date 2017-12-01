namespace KsxEventTracker.Domain.Messages.Events
{
    public class CheckpointValidationListCreated : EventBase, IEvent
    {
        public CheckpointValidationListCreated(string happeningId) : base(happeningId)
        {
        }
    }
}