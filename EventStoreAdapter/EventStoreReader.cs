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

        private EventStoreReader(Uri connectionString)
        {
            _connectionString = connectionString;
        }

        public static IEventStoreReader BuildUsing(Uri connectionString) => new EventStoreReader(connectionString);
        
        public IEventStoreSubscription SubscribeTo(
            StreamPrefix streamPrefix,
            long startPosition,
            CancellationToken token,
            IEventStoreStreamMessageReceiver receiver)
        {
            var connection = GrabSingleEventStoreConnectionFor(_connectionString).Result;

            CreateProjectionFor(streamPrefix, token);

            var catchUpSubscription = connection.SubscribeToStreamFrom(
                $"AllEvents-{streamPrefix}",
                startPosition == -1 ? null : (long?)startPosition,
                CatchUpSubscriptionSettings.Default,
                (_, x) => receiver.Receive(Convert(x)));
            
            return new EventStoreSubscription(catchUpSubscription);
        }

        private void CreateProjectionFor(StreamPrefix streamPrefix, CancellationToken token)
        {
            var sourceProjectionCreator = SourceProjectionCreator.NewFor(_connectionString);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    sourceProjectionCreator.CreateFor(streamPrefix);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
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