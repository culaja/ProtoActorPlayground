using Domain;

namespace ProtoActorAdapter.Messages
{
    internal sealed class RouteMessage
    {
        public DomainEvent Event { get; }

        public RouteMessage(DomainEvent message)
        {
            Event = message;
        }

        public string ChildActorId => Event.AggregateId;
        
        public EnqueueMessage ToEnqueueMessage() => new EnqueueMessage(Event);
    }
}