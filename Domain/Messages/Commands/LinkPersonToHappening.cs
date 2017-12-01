namespace KsxEventTracker.Domain.Messages.Commands
{
    public class LinkPersonToHappening: CommandBase, ICommand
    {
        public string PersonId { get; set; }

        public string HappeningId { get; set; }

        public string RegistrationId { get; set; }
    }
}