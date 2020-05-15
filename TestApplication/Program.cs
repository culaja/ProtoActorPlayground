using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using HttpClientAdapter;
using ProtoActorAdapter;

namespace TestApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpApplyDomainEventStrategy = HttpApplyDomainEventStrategyBuilder.New()
                .WithDestinationUri(new Uri("https://webhook.site/9705d5a2-8189-4bdc-959f-ed5540e5cdc9"))
                .WithMaxConnectionsPerServer(int.MaxValue)
                .DecorateWith(ConsoleInternalLogger.New())
                .Build();
            
            var eventsToSend = Enumerable.Range(1, 53)
                .Select(i => DomainEventBuilder.New()
                    .WithNumber(i)
                    .ForTopic($"Aggregate{i}")
                    .WithTopicVersion(i)
                    .WithData("{}")
                    .WithMetadata("{}")
                    .Using(httpApplyDomainEventStrategy)
                    .Build())
                .ToList();

            var domainEventApplier = ProtoActorDomainEventApplierBuilder.New()
                .Using(new SnapshotConfiguration(
                    new Uri("tcp://admin:changeit@localhost:1113"),
                    "TestSnapshot",
                    TimeSpan.FromSeconds(10)))
                .DecorateWith(ConsoleInternalLogger.New())
                .Build();

            var lastDispatchedDomainEvent = await domainEventApplier.ReadLastKnownDispatchedDomainEventNumber(new CancellationToken());
            foreach (var domainEvent in eventsToSend.Where(e => e.Number > lastDispatchedDomainEvent))
            {
                domainEventApplier.Pass(domainEvent);
            }
            
            Console.ReadLine();
        }
    }
}