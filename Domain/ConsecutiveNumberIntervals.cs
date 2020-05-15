using System.Collections.Generic;
using System.Linq;
using Framework;

namespace Domain
{
    public sealed class ConsecutiveNumberIntervals : ValueObject
    {
        private readonly List<ConsecutiveNumberInterval> _sortedConsecutiveNumberIntervals;

        private ConsecutiveNumberIntervals(DomainEventPosition staringPoint)
        {
            _sortedConsecutiveNumberIntervals = new List<ConsecutiveNumberInterval>
            {
                ConsecutiveNumberInterval.NewFor(DomainEventPosition.Start, staringPoint)
            };
        }
        
        public static ConsecutiveNumberIntervals New() => new ConsecutiveNumberIntervals(DomainEventPosition.Start);
        
        public static ConsecutiveNumberIntervals StartFrom(DomainEventPosition staringPoint) => new ConsecutiveNumberIntervals(staringPoint);
        
        public void Insert(DomainEventPosition number)
        {
            var newInterval = ConsecutiveNumberInterval.NewFor(number);
            Insert(newInterval);
        }

        public void Insert(ConsecutiveNumberInterval interval)
        {
            var insertPosition = FindInsertPositionFor(interval);
            
            _sortedConsecutiveNumberIntervals.Insert(insertPosition, interval);

            TryMergeRightWithMiddle(insertPosition);
            TryMergeLeftWithMiddle(insertPosition);
        }
        
        private int FindInsertPositionFor(ConsecutiveNumberInterval interval)
        {
            var binarySearchPosition = _sortedConsecutiveNumberIntervals.BinarySearch(interval);
            return binarySearchPosition >= 0 ? binarySearchPosition : ~binarySearchPosition;
        }

        private void TryMergeRightWithMiddle(int position)
        {
            if (RightIntervalExistsFor(position))
            {
                if (_sortedConsecutiveNumberIntervals[position].TryMerge(
                    _sortedConsecutiveNumberIntervals[position + 1],
                    out var mergedInterval))
                {
                    _sortedConsecutiveNumberIntervals[position] = mergedInterval;
                    _sortedConsecutiveNumberIntervals.RemoveAt(position + 1);
                }
            }
        }

        private bool RightIntervalExistsFor(int position) => position + 1 < _sortedConsecutiveNumberIntervals.Count;

        private void TryMergeLeftWithMiddle(int position)
        {
            if (LeftIntervalExistsFor(position))
            {
                if (_sortedConsecutiveNumberIntervals[position - 1].TryMerge(
                    _sortedConsecutiveNumberIntervals[position],
                    out var mergedSet))
                {
                    _sortedConsecutiveNumberIntervals[position - 1] = mergedSet;
                    _sortedConsecutiveNumberIntervals.RemoveAt(position);
                }
            }
        }

        private static bool LeftIntervalExistsFor(int position) => position > 0;

        public DomainEventPosition LargestConsecutiveNumber => _sortedConsecutiveNumberIntervals.First().Tail;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var item in _sortedConsecutiveNumberIntervals) yield return item;
        }
    }
}