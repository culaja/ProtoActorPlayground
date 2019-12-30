using System.Text;
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

        private EventStoreReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static IEventStoreReader BuildUsing(string connectionString) => new EventStoreReader(connectionString);
        
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

        private static DomainEventBuilder Convert(ResolvedEvent resolvedEvent)
        {
            var streamName = StreamName.Of(resolvedEvent.OriginalEvent.EventStreamId);
            return DomainEventBuilder.New()
                .WithNumber(resolvedEvent.Event.EventNumber)
                .ForAggregate(streamName)
                .WithAggregateVersion(resolvedEvent.OriginalEventNumber)
                .WithData(Encoding.UTF8.GetString(resolvedEvent.Event.Data))
                .WithMetadata(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
        }
    }
}