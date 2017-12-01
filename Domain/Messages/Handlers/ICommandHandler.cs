namespace KsxEventTracker.Domain.Messages.Handlers
{
    using KsxEventTracker.Domain.Messages.Commands;

    public interface ICommandHandler: IEventConsumer
    {
        bool TryExecute(ICommand command);
    }
}