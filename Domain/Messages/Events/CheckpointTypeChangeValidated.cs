namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class CheckpointTypeChangeValidated : EventBase, IEvent
    {
        public CheckpointType NewType { get; set; }

        public CheckpointTypeChangeValidated(string id, CheckpointType newType) : base(id)
        {
            this.NewType = newType;
        }
    }
}