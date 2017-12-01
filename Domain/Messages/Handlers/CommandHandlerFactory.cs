namespace KsxEventTracker.Domain.Messages.Handlers
{
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandHandlerFactory
    {
        private IEventPublisher eventpublisher;

        public CommandHandlerFactory(IEventPublisher eventpublisher, AzureTableStorageOptions options)
        {
            this.eventpublisher = eventpublisher;

            // TODO: Take dynamic approach into use when works, now just manually construct all the types
            var commandHandlerTypes = new List<Type>
            {
                typeof(CheckpointMessageHandler),
                typeof(HappeningMessageHandler),
                typeof(PersonMessageHandler),
                typeof(RegistrationMessageHandler)
            };


            // Create instance of every command handler via reflection
            // This is slightly hard to debug as assumes that there is a constructor that 
            // takes the event publisher as an argument. 
            // TODO: Should use DI container to create the instances.
            // TODO: Switch back to Assembly.GetExecutingAssembly() when it is available on .NET core
            // Does not work as this assembly is not the root, executing assembly: All = typeof(CommandHandlerFactory).GetTypeInfo().Assembly
            //All = assembly.ExecutingAssembly()
            //    .GetTypes()
            //    .Where(
            //        t =>
            //            t.GetTypeInfo().GetInterfaces().Contains(typeof(ICommandHandler))
            //            && t.GetTypeInfo().GetConstructor(Type.EmptyTypes) != null)
            All = commandHandlerTypes
                .Select(t => Activator.CreateInstance(t, eventpublisher, options) as ICommandHandler);
        }

        public IEnumerable<ICommandHandler> All { get; }
    }
}
