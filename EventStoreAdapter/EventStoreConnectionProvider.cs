using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Framework;
using static Framework.Optional<EventStore.ClientAPI.IEventStoreConnection>;

namespace EventStoreAdapter
{
    internal static class EventStoreConnectionProvider
    {
        private static readonly object SyncObject = new object();
        private static Optional<IEventStoreConnection> _eventStoreConnectionInstance = None;
        
        public static Task<IEventStoreConnection> GrabSingleEventStoreConnectionFor(string connectionString)
        {
            if (_eventStoreConnectionInstance.HasNoValue)
            {
                lock (SyncObject)
                {
                    if (_eventStoreConnectionInstance.HasNoValue)
                    {
                        _eventStoreConnectionInstance = From(EventStoreConnection.Create(GetConnectionBuilder(), new Uri(connectionString)));
                        return _eventStoreConnectionInstance.Value.ConnectAsync()
                            .ContinueWith(t => _eventStoreConnectionInstance.Value);
                    }
                }
            }

            return Task.FromResult(_eventStoreConnectionInstance.Value);
        }
        
        private static ConnectionSettings GetConnectionBuilder()
        {
            var settings = ConnectionSettings.Create()
                .KeepReconnecting();
            return settings;
        }
    }
}