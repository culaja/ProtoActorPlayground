using System;
using System.Text;
using System.Threading;
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
        private readonly IInternalLogger _logger;

        private EventStoreReader(Uri connectionString, IInternalLogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public static IEventStoreReader BuildUsing(Uri connectionString, IInternalLogger logger) 
            => new EventStoreReader(connectionString, logger);
        
        public IEventStoreSubscription SubscribeTo(
            StreamPrefix streamPrefix,
            long startPosition,
            CancellationToken token,
            IEventStoreStreamMessageReceiver receiver)
        {
            var connection = GrabSingleEventStoreConnectionFor(_connectionString).Result;

            var sourceProjectionCreator = SourceProjectionCreator.NewFor(_connectionString, _logger);
            sourceProjectionCreator.Create(streamPrefix, token);

            var catchUpSubscription = connection.SubscribeToStreamFrom(
                streamPrefix.ToStreamName(),
                startPosition == -1 ? null : (long?)startPosition,
                CatchUpSubscriptionSettings.Default,
                (_, x) => receiver.Receive(Convert(x)));
            
            return new EventStoreSubscription(catchUpSubscription);
        }

        private static DomainEventBuilder Convert(ResolvedEvent resolvedEvent) =>
            DomainEventBuilder.New()
                .WithNumber(resolvedEvent.OriginalEventNumber)
                .ForTopic(resolvedEvent.Event.EventStreamId)
                .WithTopicVersion(resolvedEvent.Event.EventNumber)
                .WithData(Encoding.UTF8.GetString(resolvedEvent.Event.Data))
                .WithMetadata(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
    }
}