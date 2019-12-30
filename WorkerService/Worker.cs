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
        private readonly StreamName _domainEventsStreamName;

        public Worker(
            ILogger<Worker> logger,
            IEventStoreReader eventStoreReader,
            IDomainEventApplier domainEventApplier,
            StreamName domainEventsStreamName)
        {
            _logger = logger;
            _eventStoreReader = eventStoreReader;
            _domainEventApplier = domainEventApplier;
            _domainEventsStreamName = domainEventsStreamName;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(Worker)} started. Subscribing to '{_domainEventsStreamName}' stream ...");
            await SubscribeAndProcessAllReceivedMessagesAsync(stoppingToken);
            _logger.LogInformation($"{nameof(Worker)} stopped.");
        }

        private async Task SubscribeAndProcessAllReceivedMessagesAsync(CancellationToken stoppingToken)
        {
            using (_eventStoreReader.SubscribeTo(_domainEventsStreamName, this))
            {
                await stoppingToken.WaitAsync();
            }
        }

        void IEventStoreStreamMessageReceiver.Receive(DomainEventBuilder domainEventBuilder) => 
            _domainEventApplier.Pass(domainEventBuilder.Build());
    }
}