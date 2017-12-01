namespace KsxEventTracker.Domain.Messages.Handlers
{
    using System.Threading.Tasks;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Repositories;

    /// <summary>
    /// Registration is not currently event sourced, but use the same 
    /// command handler pattern to receive messages from the client.
    /// </summary>
    public class RegistrationMessageHandler : ICommandHandler
    {
        private readonly RegistrationRepository repository;

        public RegistrationMessageHandler(IEventPublisher eventPublisher, AzureTableStorageOptions options)
        {
            repository = new RegistrationRepository(options);
        }

        public bool TryExecute(ICommand command) => RedirectToWhen.InvokeCommandIfHandlerFound(this, command);
        public bool TryApply(IEvent e) => RedirectToWhen.InvokeEventIfHandlerFound(this, e);

        public Task Execute(DeleteRegistration c) => this.repository.DeleteOrMarkDeleted(c.HappeningId, c.RegistrationId);
    }
}