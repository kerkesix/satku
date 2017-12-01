namespace Web.QueryModels
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    public class LinkPeopleDenormalizer: IDenormalizer
    {
        private readonly ILogger logger;

        public LinkPeopleDenormalizer(ILoggerFactory loggerFactory) {
            this.logger = loggerFactory.CreateLogger("LinkPeopleDenormalizer");
        }

        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        public void When(PersonCreated e)
        {
            QueryModelRepository.LinkPeople.Add(new LinkPerson
                                                {
                                                    PersonId = e.PersonId,
                                                    Name = e.Lastname + " " + e.Firstname,
                                                    Email = e.ContactInformation.Email,
                                                    Phone = e.ContactInformation.Phone
                                                });
        }

        public void When(PersonContactInformationUpdated e)
        {
            this.logger.LogInformation("Updating contact information for {0} - {1}", e.PersonId, e.Lastname);
            var p = Person(e.PersonId);
            p.Email = e.ContactInformation.Email;
            p.Name = e.Lastname + " " + e.Firstname;
            p.Phone = e.ContactInformation.Phone;
        }

        public void When(PersonLinkedToHappening e)
        {
            Person(e.PersonId).Happenings.Add(e.HappeningId, e.RegistrationId);
        }

        public void When(PersonUnlinkedFromHappening e)
        {
            Person(e.PersonId).Happenings.Remove(e.HappeningId);
        }

        private static LinkPerson Person(string personId)
        {
            // Let throw exception if person not found --> will retry
            var p =
                QueryModelRepository.LinkPeople.First(
                    m => m.PersonId.Equals(personId, StringComparison.OrdinalIgnoreCase));
            return p;
        }
    }
}