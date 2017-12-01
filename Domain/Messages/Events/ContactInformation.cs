namespace KsxEventTracker.Domain.Messages.Events
{
    public class ContactInformation
    {
        public ContactInformation(string phone, string email, string twitter)
        {
            this.Phone = phone;
            this.Email = email;
            this.Twitter = twitter;
        }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
    }
}