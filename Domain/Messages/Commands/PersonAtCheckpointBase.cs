namespace KsxEventTracker.Domain.Messages.Commands
{
    public abstract class PersonAtCheckpointBase: CommandBase
    {
        public string HappeningId { get; set; }
        public string CheckpointId { get; set; }
        public string PersonId { get; set; }
    }
}