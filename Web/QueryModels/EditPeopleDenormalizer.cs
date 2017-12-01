namespace Web.QueryModels
{
    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    public class EditPeopleDenormalizer: IDenormalizer
    {
        private readonly ILoggerFactory loggerFactory;

        public EditPeopleDenormalizer(ILoggerFactory loggerFactory) {
            this.loggerFactory = loggerFactory;
        }

        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        public void When(PersonCreated e)
        {
            QueryModelRepository.EditPeople.HandleCreate(e);
        }

        public void When(PersonContactInformationUpdated e)
        {
            QueryModelRepository.EditPeople.HandleUpdate(e);
        }

        public void When(PersonLinkedToHappening e)
        {
            QueryModelRepository.EditPeople.HandleLinkedToHappening(e);
        }

        public void When(PersonUnlinkedFromHappening e)
        {
            QueryModelRepository.EditPeople.HandleUnlinkedFromHappening(e);
        }

        public void When(PersonDeleted e)
        {
            QueryModelRepository.EditPeople.HandleDelete(e);
        }
    }
}