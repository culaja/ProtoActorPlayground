using Domain;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public sealed class ConsecutiveNumberIntervalsTests
    {
        [Theory]
        [InlineData(new long[] { }, 0)]
        [InlineData(new long[] { 1 }, 1)]
        [InlineData(new long[] { 1, 2 }, 2)]
        [InlineData(new long[] { 2, 1 }, 2)]
        [InlineData(new long[] { 2, 1, 100 }, 2)]
        [InlineData(new long[] { 2, 1, 4 }, 2)]
        [InlineData(new long[] { 2, 1, 4, 3 }, 4)]
        [InlineData(new long[] { 5, 10, 3, 2, 1000, 1, 2, 3, 4 }, 5)]
        [InlineData(new long[] { 1, 2, 3, 4, 10, 6, 7, 5, 9, 8, 100, 50, 30, 20, 11 }, 11)]
        public void after_applying_domain_events_last_consecutive_applied_event_is_correct(
            long[] domainEventNumbers,
            long lastConsecutiveAppliedEventNumber)
        {
            var appliedDomainEvents = ConsecutiveNumberIntervals.New();
                
            foreach (var number in domainEventNumbers) appliedDomainEvents.Insert(new DomainEventPosition(number, number, number));

            appliedDomainEvents.LargestConsecutiveNumber
                .Should().Be(new DomainEventPosition(lastConsecutiveAppliedEventNumber, lastConsecutiveAppliedEventNumber, lastConsecutiveAppliedEventNumber));
        }
    }
}