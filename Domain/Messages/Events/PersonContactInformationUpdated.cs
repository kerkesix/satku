namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonContactInformationUpdated : PersonBase, IEvent
    {
        public PersonContactInformationUpdated(
            string personId,
            string nfcId,
            string lastname, 
            string firstname, 
            string displayName, 
            ContactInformation contactInformation,
            string info)
            : base(personId, nfcId, lastname, firstname, displayName, contactInformation, info)
        {
        }
    }
}