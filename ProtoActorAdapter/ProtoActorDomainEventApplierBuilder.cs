using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Ports;
using Proto;
using Proto.Persistence;
using Proto.Persistence.EventStore;
using ProtoActorAdapter.Actors;

namespace ProtoActorAdapter
{
    public static class ProtoActorDomainEventApplierBuilder
    {
        public static async Task<IDomainEventApplier> BuildUsing(
            EventStoreConfiguration configuration,
            Uri domainEventDestinationUri)
        {
            var context = new RootContext();

            var eventStore = await BuildEventStoreUsing(configuration);

            var applierEventTrackerActorPid = BuildAppliedEventsTrackerPersistentActorUsing(
                context,
                configuration,
                eventStore);
            
            var rootActorPid = BuildRootActorUsing(
                context,
                applierEventTrackerActorPid,
                domainEventDestinationUri);
            
            return new DomainEventApplier(context, rootActorPid, applierEventTrackerActorPid);
        }

        private static async Task<ISnapshotStore> BuildEventStoreUsing(EventStoreConfiguration configuration)
        {
            var eventStoreConnection = EventStoreConnection.Create(
                configuration.ConnectionString,
                ConnectionSettings.Create().KeepReconnecting(),
                "DomainEventApplier");
            
            await eventStoreConnection.ConnectAsync();
            
            return new EventStoreProvider(eventStoreConnection);
        }

        private static PID BuildAppliedEventsTrackerPersistentActorUsing(
            RootContext rootContext,
            EventStoreConfiguration configuration,
            ISnapshotStore snapshotStore)
        {
            var props = Props.FromProducer(() => new EventMonitorActor(
                snapshotStore,
                configuration.SnapshotName,
                configuration.EventNumberPersistTrigger));
            return rootContext.Spawn(props);
        }

        private static PID BuildRootActorUsing(
            RootContext rootContext, 
            PID applierEventTrackerActorPid,
            Uri domainEventDestinationUri)
        {
            var props = Props.FromProducer(() => new RootActor(
                applierEventTrackerActorPid,
                domainEventDestinationUri));
            
            return rootContext.Spawn(props);
        }
    }
}