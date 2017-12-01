namespace Web.QueryModels
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain;
    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    public class EditHappeningsDenormalizer: IDenormalizer
    {
        private readonly ILoggerFactory loggerFactory;

        public EditHappeningsDenormalizer(ILoggerFactory loggerFactory) {
            this.loggerFactory = loggerFactory;
        }

        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        public void When(HappeningInventoryItemCreated e)
        {
            QueryModelRepository.EditHappenings.Add(new Happening { Key = e.Id });

            if (e.IsDefault)
            {
                SetAsDefault(e.Id);
            }
        }

        public void When(CoordinatePathSet e)
        {
            var happening = QueryModelRepository.EditHappenings.Find(h => h.Key == e.HappeningId);
            happening.Path = e.Path.FromGzip();
        }

        public void When(HappeningSetAsDefault e)
        {
            SetAsDefault(e.KeyOfNewDefault);
        }

        public void When(HappeningDeleted e)
        {
            QueryModelRepository.EditHappenings.RemoveAll(
                m => m.Key.Equals(e.Id, StringComparison.OrdinalIgnoreCase));
        }

        public void When(CheckpointCreated e)
        {
            var happening = Find(e.HappeningId);

            if (happening != null)
            {
                happening.CheckpointCount++;
            }
        }

        public void When(CheckpointDeleted e)
        {
            var happening = Find(e.HappeningId);

            if (happening != null)
            {
                happening.CheckpointCount--;
            }
        }

        private static Happening Find(string happeningId)
        {
            return QueryModelRepository.EditHappenings.FirstOrDefault(
                m => m.Key.Equals(happeningId, StringComparison.OrdinalIgnoreCase));
        }

        private static void SetAsDefault(string key)
        {
            QueryModelRepository.EditHappenings.ForEach(m => m.IsDefault = (m.Key == key));
        }
    }
}