using Domain;
using EventStore.ClientAPI;
using Ports;
using static EventStoreAdapter.EventStoreConnectionProvider;
using CatchUpSubscriptionSettings = EventStore.ClientAPI.CatchUpSubscriptionSettings;

namespace EventStoreAdapter
{
    public sealed class EventStoreReader : IEventStoreReader
    {
        private readonly string _connectionString;

        public EventStoreReader(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public IEventStoreSubscription SubscribeTo(StreamName streamName, IEventStoreStreamMessageReceiver receiver)
        {
            var connection = GrabSingleEventStoreConnectionFor(_connectionString).Result;

            var catchUpSubscription = connection.SubscribeToStreamFrom(
                streamName,
                null,
                CatchUpSubscriptionSettings.Default,
                (_, x) => receiver.Receive(Convert(x)));
            
            return new EventStoreSubscription(catchUpSubscription);
        }

        private static Message Convert(ResolvedEvent resolvedEvent)
        {
            var streamName = StreamName.Of(resolvedEvent.OriginalEvent.EventStreamId);
            return Message.Of(streamName, resolvedEvent.Event.Data, resolvedEvent.Event.Metadata);
        }
    }
}