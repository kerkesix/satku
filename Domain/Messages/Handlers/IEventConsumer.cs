namespace KsxEventTracker.Domain.Messages.Handlers
{
    using KsxEventTracker.Domain.Messages.Events;

    public interface IEventConsumer
    {
        bool TryApply(IEvent @event);
    }
}