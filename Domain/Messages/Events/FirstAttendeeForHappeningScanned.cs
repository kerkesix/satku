namespace KsxEventTracker.Domain.Messages.Events
{
    public class FirstAttendeeForHappeningScanned : EventBase, IEvent
    {
        public string HappeningId { get; private set; }

        public FirstAttendeeForHappeningScanned(string happeningId)
        {
            this.HappeningId = happeningId;
        }
    }
}