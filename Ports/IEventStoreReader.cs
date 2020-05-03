using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(
            SourceStream sourceStream,
            long startPosition,
            IEventStoreStreamMessageReceiver receiver);
    }
}