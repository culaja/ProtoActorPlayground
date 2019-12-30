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

namespace ProtoActorAdapter
{
    public sealed class ProtoActorDomainEventApplierBuilder
    {
        private Optional<SnapshotConfiguration> _optionalEventStoreConfiguration;
        private IInternalLogger _internalLogger = new NoInternalLogger();
        
        public static ProtoActorDomainEventApplierBuilder New() => new ProtoActorDomainEventApplierBuilder();

        public ProtoActorDomainEventApplierBuilder Using(SnapshotConfiguration configuration)
        {
            _optionalEventStoreConfiguration = configuration;
            return this;
        }

        public ProtoActorDomainEventApplierBuilder DecorateWith(IInternalLogger internalLogger)
        {
            _internalLogger = internalLogger;
            return this;
        }

        public IDomainEventApplier Build()
        {
            if (_optionalEventStoreConfiguration.HasNoValue) throw new ArgumentException("Argument is not set.", nameof(_optionalEventStoreConfiguration));
            return InternalBuild(
                _optionalEventStoreConfiguration.Value,
                _internalLogger);
        }

        private static IDomainEventApplier InternalBuild(
            SnapshotConfiguration configuration,
            IInternalLogger internalLogger)
        {
            var context = new RootContext();
            var snapshotStore = BuildEventStoreUsing(configuration);
            
            var applierEventTrackerActorPid = BuildAppliedEventsTrackerPersistentActorUsing(
                context,
                configuration,
                snapshotStore,
                internalLogger);
            
            var rootActorPid = BuildRootActorUsing(
                context,
                applierEventTrackerActorPid,
                internalLogger);
            
            return new DomainEventApplier(
                new EventMonitorActorSnapshotReader(snapshotStore, configuration.SnapshotName), 
                context,
                rootActorPid);
        }

        private static ISnapshotStore BuildEventStoreUsing(SnapshotConfiguration configuration)
        {
            var eventStoreConnection = EventStoreConnection.Create(
                configuration.ConnectionString,
                ConnectionSettings.Create().KeepReconnecting(),
                "DomainEventApplier");
            
            eventStoreConnection.ConnectAsync().Wait();
            
            return new EventStoreProvider(eventStoreConnection);
        }

        private static PID BuildAppliedEventsTrackerPersistentActorUsing(
            RootContext rootContext,
            SnapshotConfiguration configuration,
            ISnapshotStore snapshotStore,
            IInternalLogger internalLogger)
        {
            var props = Props.FromProducer(() => new EventMonitorActor(
                snapshotStore,
                configuration.SnapshotName,
                configuration.SnapshotTimeSpan));
            
            return rootContext.SpawnNamed(
                DecorateWithLogger(internalLogger, props, nameof(EventMonitorActor)),
                nameof(EventMonitorActor));
        }

        private static PID BuildRootActorUsing(
            RootContext rootContext, 
            PID applierEventTrackerActorPid,
            IInternalLogger internalLogger)
        {
            var props = Props.FromProducer(() => new RootActor(
                applierEventTrackerActorPid,
                (childProps, childActorName) => DecorateWithLogger(internalLogger, childProps, childActorName)));
            
            return rootContext.SpawnNamed(
                DecorateWithLogger(internalLogger, props, nameof(RootActor)),
                nameof(RootActor));
        }

        private static Props DecorateWithLogger(IInternalLogger internalLogger, Props props, string actorName) => props
            .WithReceiveMiddleware(ActorLoggingMiddleware.For(internalLogger, actorName).ReceiveHook)
            .WithSenderMiddleware(ActorLoggingMiddleware.For(internalLogger, actorName).SendHook);
    }
}