namespace KsxEventTracker.Domain.Messages.Commands
{
    public class DeleteHappening : CommandBase
    {
        public string Key { get; set; }
    }
}