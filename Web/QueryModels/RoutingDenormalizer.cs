namespace Web.QueryModels
{
    using System;

    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    public class RoutingDenormalizer : IDenormalizer
    {
        private readonly ILoggerFactory loggerFactory;

        public RoutingDenormalizer(ILoggerFactory loggerFactory) {
            this.loggerFactory = loggerFactory;
        }

        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        public void When(HappeningInventoryItemCreated e)
        {
            if (e.IsDefault)
            {
                QueryModelRepository.Routing.DefaultHappening = e.Id;
            }
        }

        public void When(HappeningSetAsDefault e)
        {
            QueryModelRepository.Routing.PreviousDefault = QueryModelRepository.Routing.DefaultHappening;
            QueryModelRepository.Routing.DefaultHappening = e.KeyOfNewDefault;
        }

        public void When(HappeningDeleted e)
        {
            if (e.HappeningId.Equals(QueryModelRepository.Routing.DefaultHappening, StringComparison.OrdinalIgnoreCase))
            {
                QueryModelRepository.Routing.DefaultHappening = QueryModelRepository.Routing.PreviousDefault;
            }
        }

    }
}