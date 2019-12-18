using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Proto;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter.Actors
{
    internal sealed class AggregateEventApplierActor : IActor
    {
        private readonly PID _applierEventTrackerActorPid;
        private readonly HttpDomainEventDispatcher _domainEventDispatcher;
        private readonly Queue<DomainEvent> _queue = new Queue<DomainEvent>();

        public AggregateEventApplierActor(PID applierEventTrackerActorPid, Uri destinationUri)
        {
            _applierEventTrackerActorPid = applierEventTrackerActorPid;
            _domainEventDispatcher = new HttpDomainEventDispatcher(destinationUri);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case EnqueueDomainEvent message:
                    HandleEnqueueDomainEvent(context, message);
                    break;
                case ApplyDomainEventFromQueue message:
                    HandleApplyDomainEventFromQueue(context, message.DomainEvent);
                    break;
                case DomainEventApplied message:
                    HandleDomainAppliedEvent(context, message);
                    break;
            }
        }
        
        private void HandleEnqueueDomainEvent(IContext context, EnqueueDomainEvent message)
        {
            _queue.Enqueue(message.Event);
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
                        context.Send(_applierEventTrackerActorPid, new DomainEventApplied(@event));
                    }
                    else
                    {
                        context.Send(context.Self, new ApplyDomainEventFromQueue(@event));
                    }
                });
            }
        }

        private void HandleDomainAppliedEvent(IContext context, DomainEventApplied message)
        {
            var dequeuedEvent = _queue.Dequeue();
            if (dequeuedEvent != message.DomainEvent) throw new InvalidOperationException("Events are not the same");
            if (_queue.Count > 0 ) context.Send(context.Self, new ApplyDomainEventFromQueue(_queue.Peek()));
        }
    }
}