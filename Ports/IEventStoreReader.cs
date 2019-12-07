using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(StreamName streamName, IEventStoreStreamMessageReceiver receiver);
    }
}