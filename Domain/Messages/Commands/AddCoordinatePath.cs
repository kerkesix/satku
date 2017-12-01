namespace KsxEventTracker.Domain.Messages.Commands
{
    public class AddCoordinatePath: CommandBase
    {
        public string Key { get; set; }

        public string Path { get; set; }
    }
}