using System;
using Domain;
using Microsoft.Extensions.Configuration;
using ProtoActorAdapter;

namespace WorkerService
{
    public static class ConfigurationReader
    {
        public static string EventStoreConnectionString(this IConfiguration configuration) =>
            ApplicationConfigurationFrom(configuration).EventStoreConnectionString;
        
        public static StreamName SourceStreamName(this IConfiguration configuration) => 
            StreamName.Of(ApplicationConfigurationFrom(configuration).SourceStreamName);

        public static SnapshotConfiguration SnapshotConfiguration(this IConfiguration configuration)
        {
            var applicationConfiguration = ApplicationConfigurationFrom(configuration);
            return new SnapshotConfiguration(
                applicationConfiguration.EventStoreConnectionString,
                applicationConfiguration.Snapshot.Name,
                TimeSpan.FromMilliseconds(applicationConfiguration.Snapshot.PeriodMs));
        }

        public static Uri DestinationServiceUri(this IConfiguration configuration) =>
            new Uri(configuration[$"Application:{nameof(EventStoreConnectionString)}"]);

        private static ApplicationConfiguration ApplicationConfigurationFrom(IConfiguration configuration) =>
            configuration.GetSection("Application").Get<ApplicationConfiguration>();

        private sealed class ApplicationConfiguration
        {
            public string EventStoreConnectionString { get; set; }
            public string SourceStreamName { get; set; }
            public Snapshot Snapshot { get; set; }
            public string DestinationServiceUri { get; set; }
        }

        private sealed class Snapshot
        {
            public string Name { get; set; }
            public int PeriodMs { get; set; }
        }
    }
}