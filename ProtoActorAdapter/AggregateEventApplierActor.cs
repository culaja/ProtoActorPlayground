using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Messages;

namespace ProtoActorAdapter
{
    internal sealed class AggregateEventApplierActor : IActor
    {
        private readonly Persistence _persistence;
        
        private readonly Queue<DomainEvent> _queue = new Queue<DomainEvent>();
        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        public AggregateEventApplierActor(IEventStore eventStore, string actorId)
        {
            _persistence = Persistence.WithEventSourcing(eventStore, actorId, ApplyEvent);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.Send(context.Self, new ApplyMessageFromQueue());
                    break;
                case DomainEvent message:
                    await _persistence.PersistEventAsync(message);
                    context.Send(context.Sender, new MessageEnqueued());
                    context.Send(context.Self, new ApplyMessageFromQueue());
                    break;
                case ApplyMessageFromQueue _:
                    await HandleApplyMessageFromQueue(context);
                    break;
                case MessageApplied messageApplied:
                    await _persistence.PersistEventAsync(messageApplied);
                    break;
            }
        }

        private async Task HandleApplyMessageFromQueue(IContext context)
        {
            if (_queue.TryDequeue(out var message))
            {
                if (await _messageDispatcher.Dispatch(message))
                {
                    context.Send(context.Self, new MessageApplied(message));
                }
                else
                {
                    context.Send(context.Self, new ApplyMessageFromQueue());
                }
            }
        }

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case DomainEvent message:
                    _queue.Enqueue(message);
                    break;
                case MessageApplied _:
                    _queue.Dequeue();
                    break;
            }
        }
    }
}