namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public class Quit : PersonAtCheckpointBase, ICommand
    {
        public decimal WalkedSinceLast { get; set; }
        public string Description { get; set; }

        public DateTimeOffset? QuitTimestamp { get; set; }
    }
}