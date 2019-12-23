using System;
using System.Collections.Generic;
using Framework;

namespace Domain
{
    public class EventNumber : IEquatable<EventNumber>, IComparable<EventNumber>
    {
        public long Value { get; }
        private EventNumber _head;

        public EventNumber Parent => FindParent().Item1;

        private EventNumber(long eventNumber)
        {
            Value = eventNumber;
            _head = this;
        }
        
        public static EventNumber NewEventNumber(long eventNumber) => new EventNumber(eventNumber);

        public EventNumber AttachTo(EventNumber head)
        {
            _head = head;
            return this;
        }

        public EventNumber Merge(EventNumber other)
        {
            var thisParentAndClusterSize = FindParent();
            var otherParentAndClusterSize = other.FindParent();
            return Merge(thisParentAndClusterSize, otherParentAndClusterSize);
        }

        private Tuple<EventNumber, long> FindParent()
        {
            var clusterSize = 1;
            var parent = this;
            while (!ReferenceEquals(parent, parent._head))
            {
                clusterSize++;
                parent = parent._head;
            }
            return new Tuple<EventNumber, long>(parent, clusterSize);
        }

        private static EventNumber Merge(Tuple<EventNumber, long> parentAndClusterSize1,
            Tuple<EventNumber, long> parentAndClusterSize2)
        {
            if (parentAndClusterSize1.Item2 < parentAndClusterSize2.Item2)
            {
                parentAndClusterSize1.Item1._head = parentAndClusterSize2.Item1;
                return parentAndClusterSize2.Item1;
            }
            parentAndClusterSize2.Item1._head = parentAndClusterSize1.Item1;
            return parentAndClusterSize1.Item1;
        }

        public bool Equals(EventNumber other) => Value == other?.Value;

        public override bool Equals(object obj) => obj is EventNumber other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(EventNumber other)
        {
            if (Equals(other)) return 0;
            return Value < other.Value ? -1 : 1;
        }
        
        public long OneLess() => Value - 1;
        
        public long OneMore() => Value + 1;
    }
}