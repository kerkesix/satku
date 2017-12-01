namespace KsxEventTracker.Domain.Messages.Commands
{
    public class DeletePerson: CommandBase, ICommand
    {
        public string PersonId { get; set; }
    }
}