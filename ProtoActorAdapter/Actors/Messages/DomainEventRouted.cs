namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class DomainEventRouted
    {
        public long DomainEventNumber { get; }

        public DomainEventRouted(long domainEventNumber)
        {
            DomainEventNumber = domainEventNumber;
        }
    }
}