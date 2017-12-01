namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonDeleted : EventBase, IEvent
    {
        public string PersonId { get; set; }

        public PersonDeleted(string personId)
        {
            this.PersonId = personId;
        }
    }
}