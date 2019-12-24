using Domain;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public sealed class EventNumbersUnionFindPerformanceTests
    {
        [Fact]
        public void when_numbers_are_incrementing_by_one()
        {
            var numberCount = 1000000;
            var intervals = EventNumbersUnionFind.New();
            
            for (int i = 1; i <= numberCount; ++i)
            {
                intervals.Insert(i);
                var unused = intervals.LastAppliedEventNumber;
            }
            intervals.LastAppliedEventNumber.Should().Be(numberCount);
        }

        [Fact]
        public void when_first_even_numbers_are_inserted()
        {
            var numberCount = 1000000;
            var intervals = EventNumbersUnionFind.New();
            
            for (int i = 2; i <= numberCount; i += 2)
            {
                intervals.Insert(i);
                var unused = intervals.LastAppliedEventNumber;
            }
            
            for (int i = 1; i <= numberCount; i += 2)
            {
                intervals.Insert(i);
                var unused = intervals.LastAppliedEventNumber;
            }
            
            intervals.LastAppliedEventNumber.Should().Be(numberCount);
        }
        
        [Theory]
        [InlineData(1000000, 1000, 10)]
        [InlineData(1000000, 100, 10)]
        public void simulate_some_realistic_scenario(
            long numberCount,
            int windowSize,
            int skipInterval)
        {
            var intervals = EventNumbersUnionFind.New();
            for (long i = 0; i < numberCount; i += windowSize)
            {
                for (var j = i + 1; j <= i + windowSize; ++j)
                    if (j % skipInterval != 0)
                    {
                        intervals.Insert(j);
                        var unused = intervals.LastAppliedEventNumber;
                    }
                for (var j = i + 1; j <= i + windowSize; ++j)
                    if (j % skipInterval == 0)
                    {
                        intervals.Insert(j);
                        var unused = intervals.LastAppliedEventNumber;
                    }
            }
            intervals.LastAppliedEventNumber.Should().Be(numberCount);
        }
        
    }
}