using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter.Actors
{
    internal sealed class AggregateEventApplierActor : IActor
    {
        private readonly Persistence _persistence;
        private readonly HttpDomainEventDispatcher _domainEventDispatcher;
        private readonly Queue<DomainEvent> _queue = new Queue<DomainEvent>();

        public AggregateEventApplierActor(
            IEventStore eventStore,
            string actorId,
            Uri destinationUri)
        {
            _persistence = Persistence.WithEventSourcing(eventStore, actorId, ApplyEvent);
            _domainEventDispatcher = new HttpDomainEventDispatcher(destinationUri);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    await HandleStarted(context);
                    break;
                case EnqueueDomainEvent message:
                    await HandleEnqueueDomainEvent(context, message);
                    break;
                case ApplyDomainEventFromQueue message:
                    HandleApplyDomainEventFromQueue(context, message.DomainEvent);
                    break;
                case DomainEventApplied message:
                    await HandleDomainAppliedEvent(context, message);
                    break;
            }
        }

        private async Task HandleStarted(IContext context)
        {
            await _persistence.RecoverStateAsync();
            if (_queue.Count > 0 ) context.Send(context.Self, new ApplyDomainEventFromQueue(_queue.Peek()));
        }

        private async Task HandleEnqueueDomainEvent(IContext context, EnqueueDomainEvent message)
        {
            await _persistence.PersistEventAsync(message);
            context.Send(context.Sender, new DomainEventEnqueued());
            context.Send(context.Self, new ApplyDomainEventFromQueue(message.Event));
        }

        private void HandleApplyDomainEventFromQueue(IContext context, DomainEvent @event)
        {
            if (_queue.TryPeek(out var peekedEvent) && peekedEvent == @event)
            {
                context.ReenterAfter(_domainEventDispatcher.Dispatch(@event), async result =>
                {
                    if (await result)
                    {
                        context.Send(context.Self, new DomainEventApplied(@event));
                    }
                    else
                    {
                        context.Send(context.Self, new ApplyDomainEventFromQueue(@event));
                    }
                });
            }
        }

        private async Task HandleDomainAppliedEvent(IContext context, DomainEventApplied message)
        {
            await _persistence.PersistEventAsync(new DomainEventApplied(message.DomainEvent));
            if (_queue.Count > 0 ) context.Send(context.Self, new ApplyDomainEventFromQueue(_queue.Peek()));
        }

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case EnqueueDomainEvent message:
                    Console.WriteLine($" >> {message.Event.Number}");
                    _queue.Enqueue(message.Event);
                    break;
                case DomainEventApplied message:
                    Console.WriteLine($" << {message.DomainEvent.Number}");
                    var dequeuedEvent = _queue.Dequeue();
                    if (dequeuedEvent != message.DomainEvent) throw new InvalidOperationException("Events are not the same");
                    break;
            }
        }
    }
}