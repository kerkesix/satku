namespace KsxEventTracker.Domain.Repositories
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    public class RegistrationEntity : TableEntity
    {
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Nickname { get; set; }

        public bool BeenThere { get; set; }

        public bool IsMember { get; set; }

        public string Info { get; set; }

        public DateTime? ConfirmedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}