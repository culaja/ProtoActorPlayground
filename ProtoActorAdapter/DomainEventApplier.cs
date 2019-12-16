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

        internal DomainEventApplier(IRootContext rootContext, PID rootActorId)
        {
            _rootContext = rootContext;
            _rootActorId = rootActorId;
        }

        public async Task<long> ReadLastDispatchedDomainEvent()
        {
            return -1l;
        }
        
        public void Pass(DomainEvent @event)
        {
            _rootContext.Send(_rootActorId, new RouteDomainEvent(@event));
        }
    }
}