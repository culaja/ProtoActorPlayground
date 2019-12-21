using System.Threading.Tasks;
using Domain;
using Ports;
using Proto;
using ProtoActorAdapter.Actors.Messages;

namespace ProtoActorAdapter
{
    public sealed class DomainEventApplier : IDomainEventApplier
    {
        private readonly IRootContext _rootContext;
        private readonly PID _rootActorId;
        private readonly PID _applierEventTrackerActorPid;

        internal DomainEventApplier(
            IRootContext rootContext,
            PID rootActorId,
            PID applierEventTrackerActorPid)
        {
            _rootContext = rootContext;
            _rootActorId = rootActorId;
            _applierEventTrackerActorPid = applierEventTrackerActorPid;
        }

        public async Task<long> ReadLastDispatchedDomainEvent()
        {
            var message = await _rootContext.RequestAsync<LastRoutedDomainEvent>(_applierEventTrackerActorPid,new ReadLastRoutedEvent());
            return message.Value;
        }
        
        public void Pass(DomainEvent @event)
        {
            _rootContext.Send(_rootActorId, new RouteDomainEvent(@event));
        }
    }
}