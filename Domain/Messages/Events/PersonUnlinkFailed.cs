namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonUnlinkFailed : PersonUnlinkedFromHappening
    {
        public string Reason { get; set; }

        public PersonUnlinkFailed(string happeningId, string personId, string reason)
        {
            this.Reason = reason;
            this.HappeningId = happeningId;
            this.PersonId = personId;
        }
    }
}