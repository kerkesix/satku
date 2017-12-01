namespace KsxEventTracker.Domain.Aggregates.Person
{
    using System.Collections.Generic;

    using Messages.Events;

    public class PersonState : AggregateRootStateBase
    {
        public PersonState()
        {
            Attendance = new List<string>();
        }
    
        public string NfcId { get; set; }

        public string Lastname { get; set; }

        public string Firstname { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Twitter { get; set; }

        public List<string> Attendance { get; private set; }

        public bool Deleted { get; set; }

        public void When(PersonCreated e) => UpdateContactInformation(e);
        public void When(PersonContactInformationUpdated e) => UpdateContactInformation(e);
        public void When(PersonLinkedToHappening e) => Attendance.Add(e.HappeningId);
        public void When(PersonUnlinkedFromHappening e) => Attendance.Remove(e.HappeningId);
        public void When(PersonUnlinkFailed e)  {  }
        public void When(PersonDeleted e) { Deleted = true; }

        private void UpdateContactInformation(PersonBase e)
        {
            NfcId = e.NfcId;
            Lastname = e.Lastname;
            Firstname = e.Firstname;
            DisplayName = e.DisplayName;
            Phone = e.ContactInformation.Phone;
            Email = e.ContactInformation.Email;
            Twitter = e.ContactInformation.Twitter;
        }
    }
}