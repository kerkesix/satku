namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public abstract class CommandBase: ICommand
    {
        public string Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public CommandContext Context { get; private set; }

        protected CommandBase()
        {
            this.Context = new CommandContext();
        }
    }
}