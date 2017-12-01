namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    public interface IEvent
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; }
    }
}