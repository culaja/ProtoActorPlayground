using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Messages;

namespace ProtoActorAdapter
{
    public sealed class RootActor : IActor
    {
        private readonly IEventStore _eventStore;
        private readonly Persistence _persistence;
        
        private readonly Dictionary<string, PID> _appliersByAggregateId = new Dictionary<string, PID>();
        private long _lastRoutedEvent = 0;

        public RootActor(IEventStore eventStore, string actorId)
        {
            _eventStore = eventStore;
            _persistence = Persistence.WithEventSourcing(eventStore, actorId, ApplyEvent);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    await _persistence.RecoverStateAsync();
                    break;
                case RouteMessage message:
                    if (!_appliersByAggregateId.TryGetValue(message.ChildActorId, out var applierActor))
                    {
                        applierActor = CreateAggregateEventApplierActorOf(context, message.ChildActorId);
                    }
                    
                    await context.RequestAsync<MessageEnqueued>(applierActor, message.ToEnqueueMessage());
                    await _persistence.PersistEventAsync(context.Message);
                    break;
            }
        }

        private PID CreateAggregateEventApplierActorOf(IContext context, string aggregateId)
        {
            var props = Props.FromProducer(() => new AggregateEventApplierActor(_eventStore, aggregateId));
            var applierActor = context.Spawn(props);
            _appliersByAggregateId.Add(aggregateId, applierActor);
            return applierActor;
        }

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case RouteMessage message:
                    _lastRoutedEvent = message.Event.Number;
                    break;
            }
        }
    }
}