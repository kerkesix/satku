namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonUnlinkValidated : PersonUnlinkedFromHappening
    {
        public PersonUnlinkValidated(string happeningId, string personId)
        {
            this.HappeningId = happeningId;
            this.PersonId = personId;
        }
    }
}