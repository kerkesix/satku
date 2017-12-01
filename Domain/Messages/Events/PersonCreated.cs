namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonCreated : PersonBase, IEvent
    {
        public PersonCreated(
            string personId, 
            string nfcId,
            string lastname, 
            string firstname, 
            string displayName, 
            ContactInformation contactInformation,
            string info) : base(personId, nfcId, lastname, firstname, displayName, contactInformation, info)
        {
        }
    }
}