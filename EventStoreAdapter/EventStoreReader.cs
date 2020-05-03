using System;
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
        private readonly Uri _connectionString;

        private EventStoreReader(Uri connectionString)
        {
            _connectionString = connectionString;
        }

        public static IEventStoreReader BuildUsing(Uri connectionString) => new EventStoreReader(connectionString);
        
        public IEventStoreSubscription SubscribeTo(
            SourceStream sourceStream,
            long startPosition,
            IEventStoreStreamMessageReceiver receiver)
        {
            var connection = GrabSingleEventStoreConnectionFor(_connectionString).Result;

            var catchUpSubscription = connection.SubscribeToStreamFrom(
                sourceStream,
                startPosition == -1 ? null : (long?)startPosition,
                CatchUpSubscriptionSettings.Default,
                (_, x) => receiver.Receive(Convert(x)));
            
            return new EventStoreSubscription(catchUpSubscription);
        }

        private static DomainEventBuilder Convert(ResolvedEvent resolvedEvent)
        {
            var sourceStreamName = SourceStream.Of(resolvedEvent.Event.EventStreamId);
            return DomainEventBuilder.New()
                .WithNumber(resolvedEvent.OriginalEventNumber)
                .ForAggregate(sourceStreamName)
                .WithAggregateVersion(resolvedEvent.Event.EventNumber)
                .WithData(Encoding.UTF8.GetString(resolvedEvent.Event.Data))
                .WithMetadata(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
        }
    }
}