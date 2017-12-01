namespace KsxEventTracker.Domain.Messages.Events
{
    public class AttendeeScannedAtFinishCheckpoint : EventBase, IEvent
    {
        public AttendeeScannedAtFinishCheckpoint(string happeningId, string personId)
        {
            this.HappeningId = happeningId;
            this.PersonId = personId;
        }

        public string HappeningId { get; set; }
        public string PersonId { get; set; }
    }
}