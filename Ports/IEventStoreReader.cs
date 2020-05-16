using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(
            StreamPrefix streamPrefix,
            long startPosition,
            IEventStoreStreamMessageReceiver receiver);
    }
}