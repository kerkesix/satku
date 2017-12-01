namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public class DeleteRegistration: CommandBase, ICommand
    {
        public string HappeningId { get; set; }
        public Guid RegistrationId { get; set; }
    }
}