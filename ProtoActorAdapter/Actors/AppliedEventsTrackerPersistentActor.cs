using System.Threading.Tasks;
using Domain;
using Proto;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter.Actors
{
    public sealed class AppliedEventsTrackerPersistentActor : IActor
    {
        private readonly ConsecutiveNumberIntervals _consecutiveNumberIntervals = ConsecutiveNumberIntervals.StartFrom(0);
        
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case ReadLastRoutedEvent _:
                    context.Respond(new LastRoutedDomainEvent(_consecutiveNumberIntervals.LargesConsecutiveNumber));
                    break;
                case DomainEventApplied message:
                    HandleDomainEventApplied(context, message);
                    break;
            }
            
            return Task.CompletedTask;
        }

        private void HandleDomainEventApplied(IContext context, DomainEventApplied message)
        {
            _consecutiveNumberIntervals.MarkAsApplied(message.DomainEvent.Number);
        }
    }
}