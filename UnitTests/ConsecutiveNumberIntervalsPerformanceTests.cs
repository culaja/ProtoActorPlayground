using Domain;
using Xunit;

namespace UnitTests
{
    public sealed class ConsecutiveNumberIntervalsPerformanceTests
    {
        [Fact]
        public void when_numbers_are_incrementing_by_one()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (int i = 1; i <= numberCount; ++i)
            {
                intervals.Insert(i);
            }
        }

        [Fact]
        public void when_first_odd_numbers_are_inserted()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (int i = 2; i <= numberCount / 2; i += 2)
            {
                intervals.Insert(i);
            }
            
            for (int i = 1; i <= numberCount / 2; i += 2)
            {
                intervals.Insert(i);
            }
        }
        
    }
}