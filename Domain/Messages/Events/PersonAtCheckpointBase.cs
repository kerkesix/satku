namespace KsxEventTracker.Domain.Messages.Events
{
    public abstract class PersonAtCheckpointBase : EventBase, IEvent
    {
        public string HappeningId { get; set; }
        public string CheckpointId { get; set; }
        public string PersonId { get; set; }

        protected PersonAtCheckpointBase()
        {
        }

        protected PersonAtCheckpointBase(string happeningId, string checkpointId, string personId)
        {
            this.HappeningId = happeningId;
            this.CheckpointId = checkpointId;
            this.PersonId = personId;
        }
    }
}