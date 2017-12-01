namespace KsxEventTracker.Domain.Messages.Commands
{
    public class CreateCheckpoint: CheckpointCommandBase
    {
        public string HappeningId { get; set; }
    }
}