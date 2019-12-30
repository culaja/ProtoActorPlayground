using EventStoreAdapter;
using HttpClientAdapter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoActorAdapter;

namespace WorkerService
{
    public static class ApplicationWireUp
    {
        public static void RegisterApplicationComponentsUsing(this IServiceCollection service, IConfiguration configuration) 
            => service
                .RegisterLogger(configuration)
                .RegisterEventStoreAdapterUsing(configuration)
                .RegisterSourceStreamNameUsing(configuration)
                .RegisterProtoActorAdapterUsing(configuration)
                .AddHostedService<Worker>();

        private static IServiceCollection RegisterLogger(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton<Logger>();

        private static IServiceCollection RegisterEventStoreAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service
                .AddSingleton(EventStoreReader.BuildUsing(configuration.EventStoreConnectionString()));

        private static IServiceCollection RegisterSourceStreamNameUsing(this IServiceCollection service, IConfiguration configuration)
            => service
                .AddSingleton(configuration.SourceStreamName());

        private static IServiceCollection RegisterProtoActorAdapterUsing(this IServiceCollection service,
            IConfiguration configuration)
            => service
                .AddSingleton(ProtoActorDomainEventApplierBuilder.New()
                    .Using(configuration.SnapshotConfiguration())
                    .Build());

        private static IServiceCollection RegisterHttpClientAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service
                .AddSingleton(HttpApplyDomainEventStrategyBuilder.New()
                    .WithDestinationUri(configuration.DestinationServiceUri())
                    .Build());
    }
}