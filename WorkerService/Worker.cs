using System.Threading;
using System.Threading.Tasks;
using Domain;
using Framework;
using Microsoft.Extensions.Hosting;
using Ports;

namespace WorkerService
{
    internal class Worker : BackgroundService, IEventStoreStreamMessageReceiver
    {
        private readonly IInternalLogger _logger;
        private readonly IEventStoreReader _eventStoreReader;
        private readonly IDomainEventApplier _domainEventApplier;
        private readonly IApplyDomainEventStrategy _applyDomainEventStrategy;
        private readonly StreamPrefix _domainEventsStreamPrefix;

        public Worker(
            IInternalLogger logger,
            IEventStoreReader eventStoreReader,
            IDomainEventApplier domainEventApplier,
            IApplyDomainEventStrategy applyDomainEventStrategy,
            StreamPrefix domainEventsStreamPrefix)
        {
            _logger = logger;
            _eventStoreReader = eventStoreReader;
            _domainEventApplier = domainEventApplier;
            _applyDomainEventStrategy = applyDomainEventStrategy;
            _domainEventsStreamPrefix = domainEventsStreamPrefix;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information($"{nameof(Worker)} started. Subscribing to '{_domainEventsStreamPrefix}' stream ...");
            await SubscribeAndProcessAllReceivedMessagesAsync(stoppingToken);
            _logger.Information($"{nameof(Worker)} stopped.");
        }

        private async Task SubscribeAndProcessAllReceivedMessagesAsync(CancellationToken stoppingToken)
        {
            var startPosition = await _domainEventApplier.ReadLastKnownDispatchedDomainEventNumber(stoppingToken);
            _logger.Information($"Last known dispatched domain event number is '{startPosition}'.");
            using (_eventStoreReader.SubscribeToAllEvents(startPosition, this))
            {
                await stoppingToken.WaitAsync();
            }
        }

        IDomainEvent IEventStoreStreamMessageReceiver.Receive(DomainEventBuilder domainEventBuilder)
        {
            var domainEvent = domainEventBuilder
                .Using(_applyDomainEventStrategy)
                .Build();
            
            _domainEventApplier.Pass(domainEvent);

            return domainEvent;
        }
    }
}