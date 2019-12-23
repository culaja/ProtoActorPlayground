using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using ProtoActorAdapter.Actors.Messages;
using static System.Threading.Tasks.Task;

namespace ProtoActorAdapter.Actors
{
    internal sealed class RootActor : IActor
    {
        private readonly PID _applierEventTrackerActorPid;
        private readonly Uri _destinationUri;
        
        private readonly Dictionary<string, PID> _appliersByAggregateId = new Dictionary<string, PID>();

        public RootActor(PID applierEventTrackerActorPid, Uri destinationUri)
        {
            _applierEventTrackerActorPid = applierEventTrackerActorPid;
            _destinationUri = destinationUri;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case RouteDomainEvent message:
                    var applierActor = LocateChildActorForDomainEvent(context, message);
                    context.Send(applierActor, message.ToEnqueueDomainEvent());
                    break;
            }

            return CompletedTask;
        }

        private PID LocateChildActorForDomainEvent(IContext context, RouteDomainEvent domainEvent)
        {
            if (!_appliersByAggregateId.TryGetValue(domainEvent.ChildActorId(), out var applierActor))
            {
                applierActor = CreateAggregateEventApplierActorOf(context, domainEvent.ChildActorId());
            }

            return applierActor;
        }

        private PID CreateAggregateEventApplierActorOf(IContext context, string aggregateId)
        {
            var props = Props.FromProducer(() => new AggregateEventApplierActor(_applierEventTrackerActorPid, _destinationUri));
            var applierActor = context.SpawnNamed(props, aggregateId);
            _appliersByAggregateId.Add(aggregateId, applierActor);
            return applierActor;
        }
    }
}