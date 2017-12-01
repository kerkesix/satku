namespace KsxEventTracker.Domain.Messages.Events
{
    public class PersonBase: EventBase
    {
        public string PersonId { get; }
        public string NfcId { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string DisplayName { get; }
        public string Info { get; }
        public ContactInformation ContactInformation { get; }

        public PersonBase(
            string personId,
            string nfcId,
            string lastname, 
            string firstname, 
            string displayName, 
            ContactInformation contactInformation,
            string info) 
        {
            PersonId = personId;
            NfcId = nfcId;
            Lastname = lastname;
            Firstname = firstname;
            DisplayName = displayName;
            ContactInformation = contactInformation;
            Info = info;
        }
    }
}