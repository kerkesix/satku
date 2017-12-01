namespace KsxEventTracker.Domain.Messages.Commands
{
    public class CreatePerson: CommandBase, ICommand
    {
        public string PersonId { get; set; }

        public string NfcId { get; set; }

        public string Lastname { get; set; }

        public string Firstname { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Twitter { get; set; }

        public string Info { get; set; }
    }
}