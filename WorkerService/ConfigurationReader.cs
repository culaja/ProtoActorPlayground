using System;
using Domain;
using Microsoft.Extensions.Configuration;
using ProtoActorAdapter;

namespace WorkerService
{
    public static class ConfigurationReader
    {
        public static Uri EventStoreConnectionString(this IConfiguration configuration) =>
            new Uri(ApplicationConfigurationFrom(configuration).EventStoreConnectionString);
        
        public static SourceStream SourceStreamName(this IConfiguration configuration) => 
            SourceStream.Of(ApplicationConfigurationFrom(configuration).SourceStreamName);

        public static SnapshotConfiguration SnapshotConfiguration(this IConfiguration configuration)
        {
            var applicationConfiguration = ApplicationConfigurationFrom(configuration);
            return new SnapshotConfiguration(
                new Uri(applicationConfiguration.EventStoreConnectionString),
                applicationConfiguration.Snapshot.Name,
                TimeSpan.FromMilliseconds(applicationConfiguration.Snapshot.PeriodMs));
        }

        public static Uri DestinationServiceUri(this IConfiguration configuration) =>
            new Uri(ApplicationConfigurationFrom(configuration).DestinationServiceUri);
        
        public static int MaxConnectionsPerServer(this IConfiguration configuration) =>
            ApplicationConfigurationFrom(configuration).MaxConnectionsPerServer;

        private static ApplicationConfiguration ApplicationConfigurationFrom(IConfiguration configuration) =>
            configuration.GetSection("Application").Get<ApplicationConfiguration>();

        private sealed class ApplicationConfiguration
        {
            public string EventStoreConnectionString { get; set; }
            public string SourceStreamName { get; set; }
            public Snapshot Snapshot { get; set; }
            public string DestinationServiceUri { get; set; }
            public int MaxConnectionsPerServer { get; set; }
        }

        private sealed class Snapshot
        {
            public string Name { get; set; }
            public int PeriodMs { get; set; }
        }
    }
}