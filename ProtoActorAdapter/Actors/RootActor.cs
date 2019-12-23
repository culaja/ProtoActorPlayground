using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

namespace ProtoActorAdapter.Actors
{
    internal delegate Props DecorateChildDelegate(Props props, string actorName);
    
    internal sealed class RootActor : IActor
    {
        private readonly PID _eventMonitorActorPid;
        private readonly DecorateChildDelegate _decorateChild;

        private readonly Dictionary<string, PID> _appliersByAggregateId = new Dictionary<string, PID>();

        public RootActor(
            PID eventMonitorActorPid,
            DecorateChildDelegate decorateChild)
        {
            _eventMonitorActorPid = eventMonitorActorPid;
            _decorateChild = decorateChild;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case RouteDomainEventMessage message:
                    var applierActor = LocateChildActorForDomainEvent(context, message);
                    context.Send(applierActor, message.ToEnqueueDomainEvent());
                    break;
            }

            return CompletedTask;
        }

        private PID LocateChildActorForDomainEvent(IContext context, RouteDomainEventMessage domainEventMessage)
        {
            if (!_appliersByAggregateId.TryGetValue(domainEventMessage.ChildActorId(), out var applierActor))
            {
                applierActor = CreateAggregateEventApplierActorOf(context, domainEventMessage.ChildActorId());
            }

            return applierActor;
        }

        private PID CreateAggregateEventApplierActorOf(IContext context, string aggregateId)
        {
            var props = Props.FromProducer(() => new AggregateEventApplierActor(_eventMonitorActorPid));
            var applierActor = context.SpawnNamed(_decorateChild(props, aggregateId), aggregateId);
            _appliersByAggregateId.Add(aggregateId, applierActor);
            return applierActor;
        }
    }
}