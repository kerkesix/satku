using System.Collections.Generic;
using System.Threading.Tasks;

using KsxEventTracker.Domain.Messages.Events;

namespace KsxEventTracker.Domain.Messages
{
    public interface IEventPublisher
    {
        Task Publish(IEnumerable<IEvent> events);
    }
}
