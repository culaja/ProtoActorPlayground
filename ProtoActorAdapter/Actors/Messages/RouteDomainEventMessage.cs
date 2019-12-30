using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class RouteDomainEventMessage
    {
        private readonly IDomainEvent _event;

        public RouteDomainEventMessage(IDomainEvent @event)
        {
            _event = @event;
        }

        public string ChildActorId() => _event.TopicName;
        
        public EnqueueDomainEventMessage ToEnqueueDomainEvent() => new EnqueueDomainEventMessage(_event);
        
        public override string ToString() => $"{nameof(RouteDomainEventMessage)}: Event number: {_event}.";
    }
}