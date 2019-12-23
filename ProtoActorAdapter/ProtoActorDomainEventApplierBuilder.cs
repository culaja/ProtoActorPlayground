using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Framework;
using Ports;
using Proto;
using Proto.Persistence;
using Proto.Persistence.EventStore;
using ProtoActorAdapter.Actors;
using ProtoActorAdapter.Logging;
using ILogger = Ports.ILogger;

namespace ProtoActorAdapter
{
    public sealed class ProtoActorDomainEventApplierBuilder
    {
        private Optional<EventStoreConfiguration> _optionalEventStoreConfiguration;
        private Optional<Uri> _optionalDomainEventDestinationUri;
        private ILogger _logger = new NoLogger();
        
        public static ProtoActorDomainEventApplierBuilder New() => new ProtoActorDomainEventApplierBuilder();

        public ProtoActorDomainEventApplierBuilder Using(EventStoreConfiguration configuration)
        {
            _optionalEventStoreConfiguration = configuration;
            return this;
        }

        public ProtoActorDomainEventApplierBuilder Targeting(Uri domainEventDestinationUri)
        {
            _optionalDomainEventDestinationUri = domainEventDestinationUri;
            return this;
        }

        public ProtoActorDomainEventApplierBuilder DecorateWith(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public Task<IDomainEventApplier> Build()
        {
            if (_optionalEventStoreConfiguration.HasNoValue) throw new ArgumentException("Argument is not set.", nameof(_optionalEventStoreConfiguration));
            if (_optionalDomainEventDestinationUri.HasNoValue) throw new ArgumentException("Argument is not set.", nameof(_optionalEventStoreConfiguration));
            return InternalBuild(
                _optionalEventStoreConfiguration.Value,
                _optionalDomainEventDestinationUri.Value,
                _logger);
        }

        private static async Task<IDomainEventApplier> InternalBuild(
            EventStoreConfiguration configuration,
            Uri domainEventDestinationUri,
            ILogger logger)
        {
            var context = new RootContext();
            var snapshotStore = await BuildEventStoreUsing(configuration);
            
            var applierEventTrackerActorPid = BuildAppliedEventsTrackerPersistentActorUsing(
                context,
                configuration,
                snapshotStore,
                logger);
            
            var rootActorPid = BuildRootActorUsing(
                context,
                applierEventTrackerActorPid,
                domainEventDestinationUri,
                logger);
            
            return new DomainEventApplier(
                new EventMonitorActorSnapshotReader(snapshotStore, configuration.SnapshotName), 
                context,
                rootActorPid);
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
            ISnapshotStore snapshotStore,
            ILogger logger)
        {
            var props = Props.FromProducer(() => new EventMonitorActor(
                snapshotStore,
                configuration.SnapshotName,
                configuration.SnapshotTimeSpan));
            
            return rootContext.SpawnNamed(
                DecorateWithLogger(logger, props, nameof(EventMonitorActor)),
                nameof(EventMonitorActor));
        }

        private static PID BuildRootActorUsing(
            RootContext rootContext, 
            PID applierEventTrackerActorPid,
            Uri domainEventDestinationUri,
            ILogger logger)
        {
            var props = Props.FromProducer(() => new RootActor(
                applierEventTrackerActorPid,
                domainEventDestinationUri,
                (childProps, childActorName) => DecorateWithLogger(logger, childProps, childActorName)));
            
            return rootContext.SpawnNamed(
                DecorateWithLogger(logger, props, nameof(RootActor)),
                nameof(RootActor));
        }

        private static Props DecorateWithLogger(ILogger logger, Props props, string actorName) => props
            .WithReceiveMiddleware(ActorLoggingMiddleware.For(logger, actorName).ReceiveHook)
            .WithSenderMiddleware(ActorLoggingMiddleware.For(logger, actorName).SendHook);
    }
}