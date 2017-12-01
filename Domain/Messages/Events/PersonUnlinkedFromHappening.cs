namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonUnlinkedFromHappening : PersonLinkedToHappening
    {
        public PersonUnlinkedFromHappening()
        {
        }

        public PersonUnlinkedFromHappening(string personId, string happeningId)
            : base(personId, happeningId, null)
        {
        }
    }
}