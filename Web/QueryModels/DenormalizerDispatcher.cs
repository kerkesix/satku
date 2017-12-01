namespace Web.QueryModels
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Dispatches events to all denormalizers.
    /// </summary>
    public class DenormalizerDispatcher
    {
        // Create instance of every denormalizer via reflection. Takes uglyish reference to 
        // Startup to get logger factory.
        public readonly IDenormalizer[] Denormalizers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(
                t =>
                t.GetInterfaces().Contains(typeof(IDenormalizer))
                && t.GetConstructor(new Type[] { typeof(ILoggerFactory) }) != null)
            .Select(t => Activator.CreateInstance(t, Startup.LoggerFactory) as IDenormalizer)
            .ToArray();

        /// <summary>
        /// Tries to give <param name="e">the given event</param> for all 
        /// known denormalizers. This is used mainly when replaying situation 
        /// from events. Normally events go trough <seealso cref="EventDispatcher"/>.
        /// </summary>
        /// <param name="e">The event to dispatch.</param>
        public void ReplayEvent(IEvent e)
        {
            const string LogPrefix = "Event replay (DenormalizerDispatcher.ReplayEvent) ";

            Action<IDenormalizer> replay = d =>
            {
                if (d is ISkipReplay)
                {
                    return;
                }

                // Normally denormalizer is allowed to throw and get a re-try, but in this case 
                // we need to protect the system and catch exceptions.
                try
                {
                    d.TryApply(e);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(LogPrefix + "raised an exception that was swallowed. Exception is: " + ex);
                    Debugger.Break();
                }
            };

            // A Micro-optimization to use foreach for array instead of List.ForEach, but as 
            // this method is called so frequently this actually has some performance benefits
            foreach (var d in Denormalizers)
            {
                replay(d);
            }
        }
    }
}