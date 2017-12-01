namespace KsxEventTracker.Domain.Messages.Commands
{
    public class CreateHappening: CommandBase
    {
        public string Key { get; set; }

        public bool IsDefault { get; set; }
    }
}