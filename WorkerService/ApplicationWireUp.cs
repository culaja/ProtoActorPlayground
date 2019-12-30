using EventStoreAdapter;
using HttpClientAdapter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ports;
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
                .RegisterHttpClientAdapterUsing(configuration)
                .AddHostedService<Worker>();

        private static IServiceCollection RegisterLogger(this IServiceCollection service, IConfiguration _)
            => service.AddSingleton(provider => Logger.NewUsing(provider.GetService<ILogger<Worker>>()));

        private static IServiceCollection RegisterEventStoreAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(EventStoreReader.BuildUsing(configuration.EventStoreConnectionString()));

        private static IServiceCollection RegisterSourceStreamNameUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(configuration.SourceStreamName());

        private static IServiceCollection RegisterProtoActorAdapterUsing(this IServiceCollection service,
            IConfiguration configuration)
            => service.AddSingleton(provider => ProtoActorDomainEventApplierBuilder.New()
                .Using(configuration.SnapshotConfiguration())
                .DecorateWith(provider.GetService<IInternalLogger>())
                .Build());

        private static IServiceCollection RegisterHttpClientAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(provider => HttpApplyDomainEventStrategyBuilder.New()
                .WithDestinationUri(configuration.DestinationServiceUri())
                .DecorateWith(provider.GetService<IInternalLogger>())
                .Build());
    }
}