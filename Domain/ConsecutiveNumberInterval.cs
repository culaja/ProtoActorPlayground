using System;

namespace Domain
{
    public struct ConsecutiveNumberInterval : IEquatable<ConsecutiveNumberInterval>, IComparable<ConsecutiveNumberInterval>
    {
        public static ConsecutiveNumberInterval FromOneTo(DomainEventPosition number) => NewFor(new DomainEventPosition(1, 1, 1), number);
        
        public DomainEventPosition Head { get; }
        
        public DomainEventPosition Tail { get; }

        private ConsecutiveNumberInterval(DomainEventPosition head, DomainEventPosition tail)
        {
            Head = head;
            Tail = tail;
        }
        
        public static ConsecutiveNumberInterval NewFor(DomainEventPosition head, DomainEventPosition tail) => 
            head.LogicalPosition > tail.LogicalPosition
                ? throw new ArgumentException($"Head '{head}' can't be greater than tail '{tail}'.") 
                : new ConsecutiveNumberInterval(head, tail);
        
        public static ConsecutiveNumberInterval NewFor(DomainEventPosition number) => new ConsecutiveNumberInterval(number, number);

        public int CompareTo(ConsecutiveNumberInterval other)
        {
            if (Tail.LogicalPosition < other.Head.LogicalPosition || (Head.LogicalPosition < other.Head.LogicalPosition && Tail.LogicalPosition > other.Head.LogicalPosition)) return -1;
            if (Head.LogicalPosition > other.Tail.LogicalPosition || (Tail.LogicalPosition > other.Tail.LogicalPosition && Head.LogicalPosition < other.Tail.LogicalPosition)) return 1;
            return 0;
        }

        public bool TryMerge(ConsecutiveNumberInterval other, out ConsecutiveNumberInterval value)
        {
            if (ArePossibleToMerge(other))
            {
                value = new ConsecutiveNumberInterval(
                    Head.LogicalPosition < other.Head.LogicalPosition ? Head : other.Head,
                    Tail.LogicalPosition > other.Tail.LogicalPosition ? Tail : other.Tail);
                return true;
            }

            value = default;
            return false;
        }

        private bool ArePossibleToMerge(ConsecutiveNumberInterval other) => 
            Head.LogicalPosition <= other.Tail.LogicalPosition + 1 && other.Head.LogicalPosition <= Tail.LogicalPosition + 1;

        public bool Equals(ConsecutiveNumberInterval other)
        {
            return Head.LogicalPosition == other.Head.LogicalPosition && Tail.LogicalPosition == other.Tail.LogicalPosition;
        }

        public override bool Equals(object obj)
        {
            return obj is ConsecutiveNumberInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Head.GetHashCode() * 397) ^ Tail.GetHashCode();
            }
        }

        public override string ToString() => $"{Head} - {Tail}";
    }
}