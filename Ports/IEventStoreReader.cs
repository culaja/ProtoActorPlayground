using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(
            StreamName streamName,
            long startPosition,
            IEventStoreStreamMessageReceiver receiver);
    }
}