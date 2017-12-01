namespace KsxEventTracker.Domain.Aggregates
{
    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;

    public class AggregateRootStateBase 
    {
        public void Apply<TExactStateType>(IEvent @event) where TExactStateType: class 
        {
            RedirectToWhen.InvokeEvent(this as TExactStateType, @event);
        }
    }
}