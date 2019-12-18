using System;

namespace Domain
{
    public struct ConsecutiveNumberInterval : IEquatable<ConsecutiveNumberInterval>, IComparable<ConsecutiveNumberInterval>
    {
        public static ConsecutiveNumberInterval FromOneTo(long number) => NewFor(1, number);
        
        public long Head { get; }
        
        public long Tail { get; }

        private ConsecutiveNumberInterval(long head, long tail)
        {
            Head = head;
            Tail = tail;
        }
        
        public static ConsecutiveNumberInterval NewFor(long head, long tail) => 
            head > tail
                ? throw new ArgumentException($"Head '{head}' can't be greater than tail '{tail}'.") 
                : new ConsecutiveNumberInterval(head, tail);
        
        public static ConsecutiveNumberInterval NewFor(long number) => new ConsecutiveNumberInterval(number, number);

        public int CompareTo(ConsecutiveNumberInterval other)
        {
            if (Tail < other.Head || (Head < other.Head && Tail > other.Head)) return -1;
            if (Head > other.Tail || (Tail > other.Tail && Head < other.Tail)) return 1;
            return 0;
        }

        public bool TryMerge(ConsecutiveNumberInterval other, out ConsecutiveNumberInterval value)
        {
            if (ArePossibleToMerge(other))
            {
                value = new ConsecutiveNumberInterval(
                    Math.Min(Head, other.Head), 
                    Math.Max(Tail, other.Tail));
                return true;
            }

            value = default;
            return false;
        }

        private bool ArePossibleToMerge(ConsecutiveNumberInterval other) => 
            Head <= other.Tail + 1 && other.Head <= Tail + 1;

        public bool Equals(ConsecutiveNumberInterval other)
        {
            return Head == other.Head && Tail == other.Tail;
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