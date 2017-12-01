namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public class RegistrationCommand
    {
        public Guid Id { get; set; }

        public string Happening { get; set; }

        public DateTime Time { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool BeenThere { get; set; }

        public string EmailValidationUrl { get; set; }

        public string Nickname { get; set; }

        public string Info { get; set; }

        public bool IsMember { get; set; }
    }
}