using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Proto;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

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

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case EnqueueDomainEventMessage message:
                    HandleEnqueueDomainEvent(context, message);
                    break;
                case ApplyDomainEventFromQueueMessage message:
                    HandleApplyDomainEventFromQueue(context, message.DomainEvent);
                    break;
                case DomainEventAppliedMessage message:
                    HandleDomainAppliedEvent(context, message);
                    break;
            }
            
            return CompletedTask;
        }
        
        private void HandleEnqueueDomainEvent(IContext context, EnqueueDomainEventMessage message)
        {
            _queue.Enqueue(message.Event);
            context.Send(context.Self, new ApplyDomainEventFromQueueMessage(message.Event));
        }

        private void HandleApplyDomainEventFromQueue(IContext context, DomainEvent @event)
        {
            if (_queue.TryPeek(out var peekedEvent) && peekedEvent == @event)
            {
                context.ReenterAfter(_domainEventDispatcher.Dispatch(@event), async result =>
                {
                    if (await result)
                    {
                        var message = new DomainEventAppliedMessage(@event);
                        context.Send(context.Self, message);
                        context.Send(_applierEventTrackerActorPid, message);
                    }
                    else
                    {
                        context.Send(context.Self, new ApplyDomainEventFromQueueMessage(@event));
                    }
                });
            }
        }

        private void HandleDomainAppliedEvent(IContext context, DomainEventAppliedMessage message)
        {
            var dequeuedEvent = _queue.Dequeue();
            if (dequeuedEvent != message.DomainEvent) throw new InvalidOperationException("Events are not the same");
            if (_queue.Count > 0 ) context.Send(context.Self, new ApplyDomainEventFromQueueMessage(_queue.Peek()));
        }
    }
}