using System.Threading;
using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeTo(
            StreamPrefix streamPrefix,
            long startPosition,
            CancellationToken token,
            IEventStoreStreamMessageReceiver receiver);
    }
}