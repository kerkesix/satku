namespace KsxEventTracker.Domain.Messages.Events
{
    public class AttendeeStarted : EventBase, IEvent
    {
        public AttendeeStarted(string happeningId, string personId)
        {
            this.HappeningId = happeningId;
            this.PersonId = personId;
        }

        public string HappeningId { get; set; }
        public string PersonId { get; set; }
    }
}