using System;
using Domain;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public sealed class ConsecutiveNumberIntervalTests
    {
        [Theory]
        [InlineData(0, 3, 3, 3, 0, 3)]
        [InlineData(0, 5, 6, 10, 0, 10)]
        [InlineData(0, 6, 0, 20, 0, 20)]
        [InlineData(4, 7, 7, 20, 4, 20)]
        [InlineData(0, 0, 0, 0, 0, 0)]
        public void when_merging_is_possible_returns_correct_set(
            long head1,
            long tail1,
            long head2,
            long tail2,
            long mergedHead,
            long mergedTail)
        {
            var isMerged = ConsecutiveNumberInterval.NewFor(head1, tail1)
                .TryMerge(ConsecutiveNumberInterval.NewFor(head2, tail2),
                    out var mergedSet);

            isMerged.Should().BeTrue();
            mergedSet.Should().Be(ConsecutiveNumberInterval.NewFor(mergedHead, mergedTail));
        }
        
        [Theory]
        [InlineData(0, 3, 5, 10)]
        [InlineData(0, 0, 2, 2)]
        public void when_merging_is_not_possible_returns_false(
            long head1,
            long tail1,
            long head2,
            long tail2)
        {
            var isMerged = ConsecutiveNumberInterval.NewFor(head1, tail1)
                .TryMerge(ConsecutiveNumberInterval.NewFor(head2, tail2),
                    out _);

            isMerged.Should().BeFalse();
        }

        [Fact]
        public void throws_ArgumentException_if_head_is_greater_then_tail() =>
            Assert.Throws<ArgumentException>(() => ConsecutiveNumberInterval.NewFor(2, 1));

    }
}