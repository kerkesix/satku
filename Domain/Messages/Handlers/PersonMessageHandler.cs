namespace KsxEventTracker.Domain.Messages.Handlers
{
    using System.Threading.Tasks;
    using KsxEventTracker.Domain.Aggregates.Person;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Repositories;

    public class PersonMessageHandler : ICommandHandler
    {
        private readonly AggregateRepository<Person, PersonState> personRepository;

        public PersonMessageHandler(IEventPublisher eventPublisher, AzureTableStorageOptions options)
        {
            personRepository = new AggregateRepository<Person, PersonState>(eventPublisher, options);
        }

        public bool TryExecute(ICommand command) => RedirectToWhen.InvokeCommandIfHandlerFound(this, command);
        public bool TryApply(IEvent e) => RedirectToWhen.InvokeEventIfHandlerFound(this, e);

        public Task Execute(CreatePerson c) => this.personRepository.CreateAndExecute(
                c.PersonId,
                person => person.Create(
                    c.PersonId,
                    c.NfcId,
                    c.Lastname,
                    c.Firstname,
                    c.DisplayName,
                    new ContactInformation(c.Phone, c.Email, c.Twitter),
                    c.Info,
                    c.Timestamp));

        public Task Execute(UpdateContactInformation c) => this.personRepository.Execute(
                    c.PersonId,
                    person =>
                        person.UpdateContactInformation(
                            c.PersonId,
                            c.NfcId,
                            c.Lastname,
                            c.Firstname,
                            c.DisplayName,
                            new ContactInformation(c.Phone, c.Email, c.Twitter),
                            c.Info,
                            c.Timestamp));

        public Task Execute(LinkPersonToHappening c) =>
            // TODO: Needs a way to automate timestamp & other context setting, as most events should have timestamp from the command
            this.personRepository.Execute(
                    c.PersonId,
                    person => person.LinkToHappening(c.PersonId, c.HappeningId, c.RegistrationId, c.Timestamp));

        public Task Execute(DeletePerson c) => this.personRepository.Execute(
                    c.PersonId,
                    person => person.DeletePerson(c.PersonId));

        /// <summary>
        /// This is called after unlink is validated at happening aggregate.
        /// </summary>
        /// <param name="e"></param>
        public Task When(PersonUnlinkValidated e) => this.personRepository.Execute(
                    e.PersonId,
                    person => person.UnlinkFromHappening(e.PersonId, e.HappeningId));
    }
}