namespace Domain
{
    public readonly struct DomainEventPosition
    {
        public static readonly DomainEventPosition Start = new DomainEventPosition(0L, 0L, 0L);
        
        public long LogicalPosition { get; }
        public long PhysicalCommitPosition { get; }
        public long PhysicalPreparePosition { get; }

        public DomainEventPosition(
            long logicalPosition,
            long physicalCommitPosition,
            long physicalPreparePosition)
        {
            LogicalPosition = logicalPosition;
            PhysicalCommitPosition = physicalCommitPosition;
            PhysicalPreparePosition = physicalPreparePosition;
        }
        
        public override int GetHashCode()
        {
            return LogicalPosition.GetHashCode();
        }

        public static bool operator ==(DomainEventPosition a, DomainEventPosition b)
        {
            return a.LogicalPosition == b.LogicalPosition &&
                a.PhysicalCommitPosition == b.PhysicalCommitPosition &&
                a.PhysicalPreparePosition == b.PhysicalPreparePosition;
        }

        public static bool operator !=(DomainEventPosition a, DomainEventPosition b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"{nameof(LogicalPosition)}: {LogicalPosition}, {nameof(PhysicalCommitPosition)}: {PhysicalCommitPosition}, {nameof(PhysicalPreparePosition)}: {PhysicalPreparePosition}";
        }
    }
}