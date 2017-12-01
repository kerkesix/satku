namespace KsxEventTracker.Domain.Messages.Events
{
    public class AttendeeScannedAtStartCheckpoint : EventBase, IEvent
    {
        public AttendeeScannedAtStartCheckpoint(string happeningId, string personId)
        {
            this.HappeningId = happeningId;
            this.PersonId = personId;
        }

        public string HappeningId { get; set; }
        public string PersonId { get; set; }
    }
}