using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter.Actors
{
    internal sealed class RootActor : IActor
    {
        private readonly IEventStore _eventStore;
        private readonly string _streamsPrefix;
        private readonly Uri _destinationUri;
        private readonly Persistence _persistence;
        
        private readonly Dictionary<string, PID> _appliersByAggregateId = new Dictionary<string, PID>();
        private long _lastRoutedEvent = -1;

        public RootActor(
            IEventStore eventStore,
            string streamsPrefix,
            Uri destinationUri)
        {
            _eventStore = eventStore;
            _streamsPrefix = streamsPrefix;
            _destinationUri = destinationUri;
            _persistence = Persistence.WithEventSourcing(eventStore, $"{_streamsPrefix}_Root", ApplyEvent);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    await _persistence.RecoverStateAsync();
                    break;
                case ReadLastRoutedEvent _:
                    context.Respond(new LastRoutedDomainEvent(_lastRoutedEvent));
                    break;
                case RouteDomainEvent message:
                    var applierActor = LocateChildActorForDomainEvent(context, message);
                    await context.RequestAsync<DomainEventEnqueued>(applierActor, message.ToEnqueueDomainEvent());
                    await _persistence.PersistEventAsync(message.ToDomainEventRouted());
                    break;
            }
        }

        private PID LocateChildActorForDomainEvent(IContext context, RouteDomainEvent domainEvent)
        {
            if (!_appliersByAggregateId.TryGetValue(domainEvent.ChildActorId(), out var applierActor))
            {
                applierActor = CreateAggregateEventApplierActorOf(context, domainEvent.ChildActorId());
            }

            return applierActor;
        }

        private PID CreateAggregateEventApplierActorOf(IContext context, string aggregateId)
        {
            var props = Props.FromProducer(() => new AggregateEventApplierActor(_eventStore, $"{_streamsPrefix}_{aggregateId}", _destinationUri));
            var applierActor = context.Spawn(props);
            _appliersByAggregateId.Add(aggregateId, applierActor);
            return applierActor;
        }

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case DomainEventRouted message:
                    _lastRoutedEvent = message.DomainEventNumber;
                    break;
            }
        }
    }
}