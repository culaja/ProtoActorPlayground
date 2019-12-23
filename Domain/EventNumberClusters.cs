using System;
using System.Collections.Generic;

namespace Domain
{
    public class EventNumberClusters
    {
        private readonly Dictionary<long, long> _eventNumbersToParentsDictionary = new Dictionary<long, long>();
        private readonly Dictionary<long, long> _headsToLargestElementDictionary =new Dictionary<long, long>();

        private EventNumberClusters(long startingPoint)
        {
            _eventNumbersToParentsDictionary[0] = 0;
            for (int i = 1; i <= startingPoint; i++) _eventNumbersToParentsDictionary[i] = 0;
            _headsToLargestElementDictionary[0] = startingPoint;
            LastAppliedEventNumber = startingPoint;
        }

        public static EventNumberClusters StartFrom(long startingPoint) => new EventNumberClusters(startingPoint);

        public static EventNumberClusters New() => new EventNumberClusters(0);

        public long LastAppliedEventNumber { get; private set; }

        public void Insert(long number)
        {
            if (_eventNumbersToParentsDictionary.ContainsKey(number)) return;
            _eventNumbersToParentsDictionary[number] = number;
            DetermineMergingStrategy(number).Invoke(number);
        }

        private Action<long> DetermineMergingStrategy(long eventNumber)
        {
            var oneLessExists = _eventNumbersToParentsDictionary.ContainsKey(eventNumber - 1);
            var oneMoreExists = _eventNumbersToParentsDictionary.ContainsKey(eventNumber + 1);
            if (oneLessExists && oneMoreExists) return MergeWithBothSides;
            if (oneLessExists) return MergeWithOneLess;
            if (oneMoreExists) return MergeWithOneMore;
            return DoNotMerge;
        }

        private void MergeWithBothSides(long eventNumber)
        {
            var oneLess = eventNumber - 1;
            var oneMore = eventNumber + 1;
            var largestElement = _headsToLargestElementDictionary[ParentOf(oneMore)];
            _headsToLargestElementDictionary.Remove(ParentOf(oneLess));
            _headsToLargestElementDictionary.Remove(ParentOf(oneMore));
            
            var head = Merge(oneLess, oneMore);
            head = Merge(head, eventNumber);
            _headsToLargestElementDictionary[head] = largestElement;
            
            if (LastAppliedEventNumber == oneLess) LastAppliedEventNumber = largestElement;
        }

        private void MergeWithOneLess(long eventNumber)
        {
            var oneLess = eventNumber - 1;
            _headsToLargestElementDictionary.Remove(ParentOf(oneLess));
            var head = Merge(eventNumber, oneLess);
            _headsToLargestElementDictionary[head] = eventNumber;
            if (LastAppliedEventNumber == oneLess) LastAppliedEventNumber = eventNumber;
        }
        
        private void MergeWithOneMore(long eventNumber)
        {
            var oneMore = eventNumber + 1;
            var largestElement = _headsToLargestElementDictionary[ParentOf(oneMore)];
            _headsToLargestElementDictionary.Remove(ParentOf(oneMore));
            var head = Merge(eventNumber, oneMore);
            _headsToLargestElementDictionary[head] = largestElement;
        }
        
        private void DoNotMerge(long eventNumber)
        {
            _headsToLargestElementDictionary[eventNumber] = eventNumber;
        }

        private long ParentOf(long eventNumber) => ParentAndClusterSizeOf(eventNumber).Item1;

        private Tuple<long, long> ParentAndClusterSizeOf(long eventNumber)
        {
            var clusterSize = 1;
            var parent = _eventNumbersToParentsDictionary[eventNumber];
            while (eventNumber != parent)
            {
                clusterSize++;
                eventNumber = parent;
                parent = _eventNumbersToParentsDictionary[eventNumber];
            }
            return new Tuple<long, long>(parent, clusterSize);
        }

        private long Merge(long event1, long event2)
        {
            var (parent1, clusterSize1) = ParentAndClusterSizeOf(event1);
            var (parent2, clusterSize2) = ParentAndClusterSizeOf(event2);
            if (clusterSize1 < clusterSize2)
            {
                _eventNumbersToParentsDictionary[parent1] = parent2;
                return parent2;
            }
            _eventNumbersToParentsDictionary[parent2] = parent1;
            return parent1;
        }
    }
}