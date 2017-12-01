namespace KsxEventTracker.Domain.Messages.Commands
{
    public class DeleteCheckpoint: CommandBase
    {
        public string HappeningId { get; set; }
    }
}