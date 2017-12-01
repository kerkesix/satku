namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public abstract class ChangeScanTimeCommandBase : PersonAtCheckpointBase, ICommand
    {
        public DateTimeOffset NewTime { get; set; }
    }
}