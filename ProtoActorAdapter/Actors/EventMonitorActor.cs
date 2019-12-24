using System;
using System.Threading.Tasks;
using Domain;
using Proto;
using Proto.Persistence;
using Proto.Schedulers.SimpleScheduler;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

namespace ProtoActorAdapter.Actors
{
    public sealed class EventMonitorActor : IActor
    {
        private readonly TimeSpan _snapshotTimeSpan;
        
        private readonly Persistence _persistence;
        private readonly ISimpleScheduler _scheduler = new SimpleScheduler();
        private readonly ConsecutiveNumberIntervals _consecutiveNumberIntervals = ConsecutiveNumberIntervals.StartFrom(0);
        private long _lastSnapshottedDomainEventNumber;

        public EventMonitorActor(
            ISnapshotStore snapshotStore,
            string actorId,
            TimeSpan snapshotTimeSpan)
        {
            _snapshotTimeSpan = snapshotTimeSpan;
            _persistence = Persistence.WithSnapshotting(snapshotStore, actorId, ApplySnapshot);
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    return HandleStarted(context);
                case DomainEventAppliedMessage message:
                    HandleDomainEventApplied(context, message);
                    break;
                case TakeSnapshotOfAppliedDomainEventsMessage _:
                    return HandleTakeSnapshotOfAppliedDomainEventsMessage();
            }

            return CompletedTask;
        }

        private Task HandleStarted(IContext context)
        {
            _scheduler.ScheduleTellRepeatedly(
                _snapshotTimeSpan,
                _snapshotTimeSpan,
                context.Self,
                new TakeSnapshotOfAppliedDomainEventsMessage(),
                out _);
            return _persistence.RecoverStateAsync(); 
        }

        private void HandleDomainEventApplied(IContext context, DomainEventAppliedMessage message) => 
            _consecutiveNumberIntervals.Insert(message.DomainEvent.Number);

        private Task HandleTakeSnapshotOfAppliedDomainEventsMessage() =>
            ShouldTakeSnapshot()
                ? TakeSnapshot()
                : CompletedTask;

        private bool ShouldTakeSnapshot() => 
            _lastSnapshottedDomainEventNumber != _consecutiveNumberIntervals.LargestConsecutiveNumber;

        private Task TakeSnapshot()
        {
            _lastSnapshottedDomainEventNumber = _consecutiveNumberIntervals.LargestConsecutiveNumber;
            return _persistence.PersistSnapshotAsync(_consecutiveNumberIntervals.LargestConsecutiveNumber);
        }

        private void ApplySnapshot(Snapshot snapshot)
        {
            switch (snapshot.State)
            {
                case long largestConsecutiveNumber:
                    _consecutiveNumberIntervals.Insert(ConsecutiveNumberInterval.FromOneTo(largestConsecutiveNumber));
                    _lastSnapshottedDomainEventNumber = _consecutiveNumberIntervals.LargestConsecutiveNumber;
                    break;
                default:
                    throw new NotSupportedException($"Snapshot not supported: '{snapshot.State}'");
            }
        }
    }
}