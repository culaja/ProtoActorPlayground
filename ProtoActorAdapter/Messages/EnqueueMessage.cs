using Domain;

namespace ProtoActorAdapter.Messages
{
    internal sealed class EnqueueMessage
    {
        public DomainEvent Event { get; }

        public EnqueueMessage(DomainEvent @event)
        {
            Event = @event;
        }
    }
}