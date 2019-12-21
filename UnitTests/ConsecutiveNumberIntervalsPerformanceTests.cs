using Domain;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public sealed class ConsecutiveNumberIntervalsPerformanceTests
    {
        [Fact]
        public void simulate_best_case_scenario()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (int i = 1; i <= numberCount; ++i)
            {
                intervals.Insert(i);
            }
            
            intervals.LargestConsecutiveNumber.Should().Be(numberCount);
        }

        [Fact]
        public void simulate_worst_case_scenario()
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

            intervals.LargestConsecutiveNumber.Should().Be(numberCount);
        }

        [Fact]
        public void simulate_some_realistic_scenario()
        {
            var numberCount = 1000000;
            var intervals = ConsecutiveNumberIntervals.New();
            
            for (long i = 0; i < numberCount; i += 1000)
            {
                for (long j = i + 1; j <= i + 1000; ++j)
                {
                    if (j % 10 != 0)
                    {
                        intervals.Insert(j);    
                    }
                }

                for (long j = i + 1; j <= i + 1000; ++j)
                {
                    if (j % 10 == 0)
                    {
                        intervals.Insert(j);    
                    }
                }
            }
            
            intervals.LargestConsecutiveNumber.Should().Be(numberCount);
        }
    }
}