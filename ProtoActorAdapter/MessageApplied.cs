using Domain;

namespace ProtoActorAdapter
{
    internal sealed class MessageApplied
    {
        public DomainEvent Message { get; }

        public MessageApplied(DomainEvent message)
        {
            Message = message;
        }
    }
}