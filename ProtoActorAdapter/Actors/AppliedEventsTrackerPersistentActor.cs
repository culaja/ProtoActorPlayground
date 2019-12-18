using System;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

namespace ProtoActorAdapter.Actors
{
    public sealed class AppliedEventsTrackerPersistentActor : IActor
    {
        private readonly int _eventNumberPersistTrigger;
        private readonly Persistence _persistence;
        private readonly ConsecutiveNumberIntervals _consecutiveNumberIntervals = ConsecutiveNumberIntervals.StartFrom(0);

        public AppliedEventsTrackerPersistentActor(
            ISnapshotStore snapshotStore,
            string actorId,
            int eventNumberPersistTrigger)
        {
            _eventNumberPersistTrigger = eventNumberPersistTrigger;
            _persistence = Persistence.WithSnapshotting(snapshotStore, actorId, ApplySnapshot);
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    return _persistence.RecoverStateAsync();
                case ReadLastRoutedEvent _:
                    context.Respond(new LastRoutedDomainEvent(_consecutiveNumberIntervals.LargestConsecutiveNumber));
                    break;
                case DomainEventApplied message:
                    return HandleDomainEventApplied(context, message);
            }

            return CompletedTask;
        }

        private Task HandleDomainEventApplied(IContext context, DomainEventApplied message)
        {
            _consecutiveNumberIntervals.Insert(message.DomainEvent.Number);
            return PersistIfNeeded(message.DomainEvent.Number);
        }

        private Task PersistIfNeeded(long eventNumber)
        {
            if (eventNumber % _eventNumberPersistTrigger == 0)
                return _persistence.PersistSnapshotAsync(_consecutiveNumberIntervals.LargestConsecutiveNumber);
            return CompletedTask;
        }

        private void ApplySnapshot(Snapshot snapshot)
        {
            switch (snapshot.State)
            {
                case long largestConsecutiveNumber:
                    _consecutiveNumberIntervals.Insert(ConsecutiveNumberInterval.FromOneTo(largestConsecutiveNumber));
                    break;
                default:
                    throw new NotSupportedException($"Snapshot not supported: '{snapshot.State}'");
            }
        }
    }
}