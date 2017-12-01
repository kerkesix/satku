namespace Web.QueryModels
{
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    using KsxEventTracker.Domain.Messages.Events;

    public class Person : IPersonOwned
    {
        // Helper to avoid having this mapping information in many places. Not pedant.
        public Person(PersonCreated e)
        {
            PersonId = e.PersonId;
            NfcId = e.NfcId;
            Lastname = e.Lastname;
            Firstname = e.Firstname;
            Email = e.ContactInformation.Email;
            DisplayName = e.DisplayName;
            Phone = e.ContactInformation.Phone;
            Twitter = e.ContactInformation.Twitter;
            Info = e.Info;
        }

        public string PersonId { get; set; }
        public string NfcId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
        public string Info { get; set; }

        public HashSet<string> HappeningsAttended { get; } = new HashSet<string>();
        public HashSet<string> HappeningsCompleted { get; } = new HashSet<string>();
        public HashSet<string> HappeningsQuit { get; } = new HashSet<string>();
        public bool QuitLastTime { get; set; }

        public string GravatarHash
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Email))
                {
                    return string.Empty;
                }

                string email = this.Email.Trim();
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(email));

                var hash = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    hash.Append(result[i].ToString("x2"));
                }

                return hash.ToString();
            }
        }

        public void UpdateContactInformation(PersonContactInformationUpdated e)
        {
            NfcId = e.NfcId;
            Lastname = e.Lastname;
            Firstname = e.Firstname;
            DisplayName = e.DisplayName;
            Phone = e.ContactInformation.Phone;
            Email = e.ContactInformation.Email;
            Twitter = e.ContactInformation.Twitter;
            Info = e.Info;
        }
    }
}