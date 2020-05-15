using System;
using System.Threading.Tasks;
using Domain;
using Ports;
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
        private readonly IInternalLogger _logger;

        private readonly Persistence _persistence;
        private readonly ISimpleScheduler _scheduler = new SimpleScheduler();
        private readonly ConsecutiveNumberIntervals _consecutiveNumberIntervals = ConsecutiveNumberIntervals.StartFrom(DomainEventPosition.Start);
        private DomainEventPosition _lastSnapshottedDomainEventPosition;

        public EventMonitorActor(
            ISnapshotStore snapshotStore,
            string actorId,
            TimeSpan snapshotTimeSpan,
            IInternalLogger logger)
        {
            _snapshotTimeSpan = snapshotTimeSpan;
            _logger = logger;
            _persistence = Persistence.WithSnapshotting(snapshotStore, actorId, ApplySnapshot);
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    return HandleStarted(context);
                case DomainEventAppliedMessage message:
                    HandleDomainEventApplied(message);
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

        private void HandleDomainEventApplied(DomainEventAppliedMessage message) => 
            _consecutiveNumberIntervals.Insert(message.DomainEvent.Position);

        private Task HandleTakeSnapshotOfAppliedDomainEventsMessage() =>
            ShouldTakeSnapshot()
                ? TakeSnapshot()
                : CompletedTask;

        private bool ShouldTakeSnapshot() => 
            _lastSnapshottedDomainEventPosition != _consecutiveNumberIntervals.LargestConsecutiveNumber;

        private Task TakeSnapshot()
        {
            _lastSnapshottedDomainEventPosition = _consecutiveNumberIntervals.LargestConsecutiveNumber;
            _logger.Information($"[{nameof(EventMonitorActor)}] Creating a snapshot at largest consecutive applied event number: '{_lastSnapshottedDomainEventPosition}'.");
            return _persistence.PersistSnapshotAsync(_consecutiveNumberIntervals.LargestConsecutiveNumber);
        }

        private void ApplySnapshot(Snapshot snapshot)
        {
            switch (snapshot.State)
            {
                case DomainEventPosition largestConsecutiveNumber:
                    _consecutiveNumberIntervals.Insert(ConsecutiveNumberInterval.FromOneTo(largestConsecutiveNumber));
                    _lastSnapshottedDomainEventPosition = _consecutiveNumberIntervals.LargestConsecutiveNumber;
                    break;
                default:
                    throw new NotSupportedException($"Snapshot not supported: '{snapshot.State}'");
            }
        }
    }
}