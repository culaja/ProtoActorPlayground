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
            
            var props = Props.FromProducer(() => new RootActor(
                domainEventDestinationUri));
            
            var pid = context.Spawn(props);
            
            return new DomainEventApplier(context, pid);
        }

        private static async Task<IEventStore> BuildEventStoreUsing(EventStoreConfiguration configuration)
        {
            var eventStoreConnection = EventStoreConnection.Create(
                configuration.ConnectionString,
                ConnectionSettings.Create().KeepReconnecting(),
                "DomainEventApplier");
            
            await eventStoreConnection.ConnectAsync();
            
            return new EventStoreProvider(eventStoreConnection);
        }
    }
}