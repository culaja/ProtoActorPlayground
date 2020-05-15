using System;
using System.Text;
using Domain;
using EventStore.ClientAPI;
using Framework;
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
        
        public IEventStoreSubscription SubscribeToAllEvents(DomainEventPosition startPosition, IEventStoreStreamMessageReceiver receiver)
        {
            var connection = GrabSingleEventStoreConnectionFor(_connectionString).Result;

            var currentPosition = startPosition.LogicalPosition;
            var catchUpSubscription = connection.SubscribeToAllFrom(
                new Position(startPosition.PhysicalCommitPosition, startPosition.PhysicalPreparePosition),
                CatchUpSubscriptionSettings.Default,
                (_, x) => TryConvert(x, ref currentPosition).Map(receiver.Receive) ,
                LiveProcessingStarted,
                SubscriptionDropped,
                _connectionString.ExtractUserCredentials());
            
            return new EventStoreSubscription(catchUpSubscription);
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            Console.WriteLine(arg2);
            Console.WriteLine(arg3.Message);
        }

        private static Optional<DomainEventBuilder> TryConvert(ResolvedEvent resolvedEvent, ref long number) =>
            resolvedEvent.OriginalStreamId.StartsWith("Domain|")
                ? DomainEventBuilder.New()
                    .WithPosition(new DomainEventPosition(++number, resolvedEvent.OriginalPosition.Value.CommitPosition, resolvedEvent.OriginalPosition.Value.PreparePosition))
                    .ForTopic(resolvedEvent.OriginalStreamId)
                    .WithTopicVersion(resolvedEvent.Event.EventNumber)
                    .WithData(Encoding.UTF8.GetString(resolvedEvent.Event.Data))
                    .WithMetadata(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata))
                : Optional<DomainEventBuilder>.None;
    }
}