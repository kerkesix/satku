namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public interface ICommand
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; set; }

        CommandContext Context { get; } 
    }
}