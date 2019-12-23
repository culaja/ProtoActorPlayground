using FluentAssertions;
using Xunit;
using static Domain.EventNumber;

namespace UnitTests
{
    public class EventNumberTests
    {
        [Fact]
        public void after_merging_two_equal_groups_head_should_be_the_calling_group()
        {
            var zero = NewEventNumber(0);
            var one = NewEventNumber(1);
            one.AttachTo(zero);

            var two = NewEventNumber(2);
            var three = NewEventNumber(3);
            two.AttachTo(three);

            two.Merge(one).Should().Be(three);
        }
        
        [Fact]
        public void after_merging_two_groups_head_should_be_the_head_of_larger_group()
        {
            var zero = NewEventNumber(0);

            var one = NewEventNumber(1);
            var two = NewEventNumber(2);
            one.AttachTo(two);

            zero.Merge(one).Should().Be(two);
        }
    }
}