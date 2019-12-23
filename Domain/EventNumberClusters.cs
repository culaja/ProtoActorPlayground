using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using static Domain.EventNumber;

namespace Domain
{
    public class EventNumberClusters
    {
        private readonly Dictionary<long, EventNumber> _eventNumbers = new Dictionary<long, EventNumber>();
        private readonly Dictionary<EventNumber, long> _headsToLargestElementDictionary =
            new Dictionary<EventNumber, long>();

        private EventNumberClusters(long startingPoint)
        {
            var zero = NewEventNumber(0);
            _headsToLargestElementDictionary[zero] = startingPoint;
            _eventNumbers[0] = zero;
            for (var i = 1; i <= startingPoint; i++) _eventNumbers[i] = NewEventNumber(i).AttachTo(zero);
            LastAppliedEventNumber = startingPoint;
        }

        public static EventNumberClusters StartFrom(long startingPoint) => new EventNumberClusters(startingPoint);

        public static EventNumberClusters New() => new EventNumberClusters(0);

        public long LastAppliedEventNumber { get; private set; }

        public void Insert(long number)
        {
            Insert(NewEventNumber(number));
        }

        public void Insert(EventNumber eventNumber)
        {
            if (_eventNumbers.ContainsKey(eventNumber.Value)) return;
            _eventNumbers[eventNumber.Value] = eventNumber;
            DetermineMergingStrategy(eventNumber).Invoke(eventNumber);
        }

        private Action<EventNumber> DetermineMergingStrategy(EventNumber eventNumber)
        {
            if (_eventNumbers.ContainsKey(eventNumber.OneLess()) && _eventNumbers.ContainsKey(eventNumber.OneMore()))
                return MergeWithBothSides;
            if (_eventNumbers.ContainsKey(eventNumber.OneLess()))
                return MergeWithOneLess;
            if (_eventNumbers.ContainsKey(eventNumber.OneMore()))
                return MergeWithOneMore;
            return DoNotMerge;
        }

        private void MergeWithBothSides(EventNumber eventNumber)
        {
            var oneLess = _eventNumbers[eventNumber.OneLess()];
            var oneMore = _eventNumbers[eventNumber.OneMore()];
            var largestElement = _headsToLargestElementDictionary[oneMore.Parent];
            _headsToLargestElementDictionary.Remove(oneLess.Parent);
            _headsToLargestElementDictionary.Remove(oneMore.Parent);
            
            var head = oneLess.Merge(oneMore);
            head = head.Merge(eventNumber);
            _headsToLargestElementDictionary[head] = largestElement;
            if (LastAppliedEventNumber == oneLess.Value) LastAppliedEventNumber = largestElement;
        }

        private void MergeWithOneLess(EventNumber eventNumber)
        {
            var oneLess = _eventNumbers[eventNumber.OneLess()];
            _headsToLargestElementDictionary.Remove(oneLess.Parent);
            var head = eventNumber.Merge(oneLess);
            _headsToLargestElementDictionary[head] = eventNumber.Value;
            if (LastAppliedEventNumber == oneLess.Value) LastAppliedEventNumber = eventNumber.Value;
        }
        
        private void MergeWithOneMore(EventNumber eventNumber)
        {
            var oneMore = _eventNumbers[eventNumber.OneMore()];
            var largestElement = _headsToLargestElementDictionary[oneMore.Parent];
            _headsToLargestElementDictionary.Remove(oneMore.Parent);
            var head = eventNumber.Merge(oneMore);
            _headsToLargestElementDictionary[head] = largestElement;
        }
        
        private void DoNotMerge(EventNumber eventNumber)
        {
            _headsToLargestElementDictionary[eventNumber] = eventNumber.Value;
        }
    }
}