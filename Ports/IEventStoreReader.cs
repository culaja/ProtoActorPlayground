using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(
            SourceStreamName sourceStreamName,
            long startPosition,
            IEventStoreStreamMessageReceiver receiver);
    }
}