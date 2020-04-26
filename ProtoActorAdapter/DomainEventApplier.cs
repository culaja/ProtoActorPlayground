using System.Threading;
using System.Threading.Tasks;
using Domain;
using Ports;
using Proto;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter
{
    internal sealed class DomainEventApplier : IDomainEventApplier
    {
        private readonly EventMonitorActorSnapshotReader _eventMonitorActorSnapshotReader;
        private readonly IRootContext _rootContext;
        private readonly PID _rootActorId;

        internal DomainEventApplier(
            EventMonitorActorSnapshotReader eventMonitorActorSnapshotReader,
            IRootContext rootContext,
            PID rootActorId)
        {
            _eventMonitorActorSnapshotReader = eventMonitorActorSnapshotReader;
            _rootContext = rootContext;
            _rootActorId = rootActorId;
        }

        public Task<long> ReadLastKnownDispatchedDomainEventNumber(CancellationToken token)
        {
            return _eventMonitorActorSnapshotReader.ReadLastSnapshot(token);
        }
        
        public void Pass(IDomainEvent @event)
        {
            _rootContext.Send(_rootActorId, new RouteDomainEventMessage(@event));
        }

        public void Dispose()
        {
        }
    }
}