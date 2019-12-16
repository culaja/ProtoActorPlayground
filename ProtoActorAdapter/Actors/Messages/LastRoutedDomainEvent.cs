namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class LastRoutedDomainEvent
    {
        public long Value { get; }

        public LastRoutedDomainEvent(long value)
        {
            Value = value;
        }
    }
}