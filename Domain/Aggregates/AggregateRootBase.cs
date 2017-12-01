namespace KsxEventTracker.Domain.Aggregates
{
    using System;

    using KsxEventTracker.Domain.Messages.Events;

    // Adapted from http://cqrsguide.com/doc:aggregate
    // See also: http://masak.org/carl/ddd-cqrs-wiki/faq
    public class AggregateRootBase<T> where T : AggregateRootStateBase
    {
        public T State { get; private set; }

        // private variable that holds an observer delegate
        readonly Action<IEvent> addToUnitOfWork;

        public AggregateRootBase(Action<IEvent> collectEvent, T state)
        {
            this.addToUnitOfWork = collectEvent;
            this.State = state;
        }

        protected void Apply(IEvent @event)
        {
            // Pass the event to state (and let it update itself)
            this.State.Apply<T>(@event);

            this.addToUnitOfWork(@event);
        }
    }
}