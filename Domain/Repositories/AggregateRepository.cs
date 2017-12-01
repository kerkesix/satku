namespace KsxEventTracker.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Messages.Events;

    using Messages;

    public class AggregateRepository<TAggregate, TState>
        where TAggregate : Aggregates.AggregateRootBase<TState>
        where TState : Aggregates.AggregateRootStateBase, new()
    {
        private readonly EventStream stream;
        private IEventPublisher eventPublisher;

        public AggregateRepository(IEventPublisher eventPublisher, AzureTableStorageOptions options)
        {
            this.eventPublisher = eventPublisher;
            stream = new EventStream(options);
        }

        public Task Execute(string id, Action<TAggregate> when)
        {
            var streamName = StreamName(id);
            var givenAsync = this.stream.ReadAllEvents(streamName);
            
            // TODO: Avoid waiting, make async all the way
            Task.WaitAll(givenAsync); 
            
            var given = givenAsync.Result;

            // load state from the event history
            // or, if you have snapshot - load it here first
            // we will not do the latter here
            var state = new TState();

            foreach (var e in given)
            {
                state.Apply<TState>(e);
            }

            return ExecuteAgainstAggregate(when, state, streamName);
        }

        public Task CreateAndExecute(string id, Action<TAggregate> when)
        {
            // Ensure that this is not a duplicate
            var streamName = StreamName(id);
            var givenAsync = this.stream.ReadAllEvents(streamName);
            
            // TODO: Avoid waiting, make async all the way            
            Task.WaitAll(givenAsync); 
            
            var given = givenAsync.Result;

            if (given.Any())
            {
                throw new DuplicateAggregateIdException();
            }

            var state = new TState();

            return ExecuteAgainstAggregate(when, state, streamName);
        }

        private Task ExecuteAgainstAggregate(Action<TAggregate> when, TState state, string streamName)
        {
            var thenEvents = new List<IEvent>();

            // Create a new instance of the aggregate root
            var ctor = TypeFactory.GetCtor<Action<IEvent>, TState, TAggregate>();
            var aggegateRoot = ctor(thenEvents.Add, state);

            // execute actual command
            when(aggegateRoot);

            // do something with the events that were produced.
            // for example - append them to the history and then 
            // publish in async or do both at once and face 2PC
            return stream.AppendEvents(streamName, thenEvents)
                .ContinueWith(
                    entities => eventPublisher.Publish(thenEvents), 
                    TaskContinuationOptions.NotOnFaulted);
        }

        private static string StreamName(string id)
        {
            return typeof(TAggregate).Name + "-" + id;
        }
    }
}