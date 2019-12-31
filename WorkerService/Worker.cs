using System.Threading;
using System.Threading.Tasks;
using Domain;
using Framework;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ports;

namespace WorkerService
{
    internal class Worker : BackgroundService, IEventStoreStreamMessageReceiver
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventStoreReader _eventStoreReader;
        private readonly IDomainEventApplier _domainEventApplier;
        private readonly IApplyDomainEventStrategy _applyDomainEventStrategy;
        private readonly SourceStreamName _domainEventsSourceStreamName;

        public Worker(
            ILogger<Worker> logger,
            IEventStoreReader eventStoreReader,
            IDomainEventApplier domainEventApplier,
            IApplyDomainEventStrategy applyDomainEventStrategy,
            SourceStreamName domainEventsSourceStreamName)
        {
            _logger = logger;
            _eventStoreReader = eventStoreReader;
            _domainEventApplier = domainEventApplier;
            _applyDomainEventStrategy = applyDomainEventStrategy;
            _domainEventsSourceStreamName = domainEventsSourceStreamName;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(Worker)} started. Subscribing to '{_domainEventsSourceStreamName}' stream ...");
            await SubscribeAndProcessAllReceivedMessagesAsync(stoppingToken);
            _logger.LogInformation($"{nameof(Worker)} stopped.");
        }

        private async Task SubscribeAndProcessAllReceivedMessagesAsync(CancellationToken stoppingToken)
        {
            var startPosition = await _domainEventApplier.ReadLastKnownDispatchedDomainEventNumber();
            using (_eventStoreReader.SubscribeTo(_domainEventsSourceStreamName, startPosition,this))
            {
                await stoppingToken.WaitAsync();
            }
        }

        void IEventStoreStreamMessageReceiver.Receive(DomainEventBuilder domainEventBuilder) => 
            _domainEventApplier.Pass(domainEventBuilder
                .Using(_applyDomainEventStrategy)
                .Build());
    }
}