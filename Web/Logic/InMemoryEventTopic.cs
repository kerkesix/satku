using KsxEventTracker.Domain;
using KsxEventTracker.Domain.Messages;
using KsxEventTracker.Domain.Messages.Events;
using KsxEventTracker.Domain.Messages.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Web.Logic
{
    public interface IEventTopic
    {
        void Subscribe(IEventConsumer handler);
    }

    public class InMemoryEventTopic : IEventTopic, IEventPublisher
    {
        private List<IEventConsumer> subscribers = new List<IEventConsumer>();
        private readonly ILogger logger;

        public InMemoryEventTopic(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger("InMemoryEventTopic");
        }

        public Task Publish(IEnumerable<IEvent> events)
        {
            this.logger.LogInformation("Received {0} events", events.Count());

            // Events must be handled in order
            foreach (var e in events)
            {
                this.logger.LogInformation("Dispatching event {0}", e);

                // ...but handlers can be run in parallel
                subscribers.AsParallel().ForAll(consumer =>
                {
                    this.logger.LogInformation("{0}: {1}, Id '{2}'", consumer.GetType().Name, e.GetType().FullName, e.Id);
                    consumer.TryApply(e);
                });
            }

            // Hack, should return real value, this is legacy from previous implementation
            return Task.CompletedTask;
        }

        public void Subscribe(IEventConsumer handler)
        {
            if (!subscribers.Contains(handler))
            {
                subscribers.Add(handler);
            }
        }
    }
}

