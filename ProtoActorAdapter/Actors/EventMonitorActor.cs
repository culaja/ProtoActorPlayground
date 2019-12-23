using System;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

namespace ProtoActorAdapter.Actors
{
    public sealed class EventMonitorActor : IActor
    {
        private readonly int _snapshotLimit;
        private readonly Persistence _persistence;
        private readonly ConsecutiveNumberIntervals _consecutiveNumberIntervals = ConsecutiveNumberIntervals.StartFrom(0);
        private long _receivedEventCount;

        public EventMonitorActor(
            ISnapshotStore snapshotStore,
            string actorId,
            int snapshotLimit)
        {
            _snapshotLimit = snapshotLimit;
            _persistence = Persistence.WithSnapshotting(snapshotStore, actorId, ApplySnapshot);
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    return _persistence.RecoverStateAsync();
                case DomainEventAppliedMessage message:
                    return HandleDomainEventApplied(context, message);
            }

            return CompletedTask;
        }

        private Task HandleDomainEventApplied(IContext context, DomainEventAppliedMessage message)
        {
            IncrementReceivedEventCount();
            _consecutiveNumberIntervals.Insert(message.DomainEvent.Number);
            return PersistIfNeeded();
        }

        private void IncrementReceivedEventCount() => _receivedEventCount++;

        private Task PersistIfNeeded()
        {
            if (ShouldPersistSnapshot)
                return _persistence.PersistSnapshotAsync(_consecutiveNumberIntervals.LargestConsecutiveNumber);
            return CompletedTask;
        }

        private bool ShouldPersistSnapshot => _receivedEventCount % _snapshotLimit == 0; 

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