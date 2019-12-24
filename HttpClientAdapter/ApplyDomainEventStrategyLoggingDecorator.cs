using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Domain;
using Ports;

namespace HttpClientAdapter
{
    internal sealed class ApplyDomainEventStrategyLoggingDecorator : IApplyDomainEventStrategy
    {
        private readonly ILogger _logger;
        private readonly IApplyDomainEventStrategy _next;

        public ApplyDomainEventStrategyLoggingDecorator(ILogger logger, IApplyDomainEventStrategy next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task<bool> TryApply(IDomainEvent domainEvent)
        {
            try
            {
                _logger.Verbose($"Applying domain event with number {domainEvent.Number} ...");
                var result = await _next.TryApply(domainEvent);
                _logger.Verbose($"Domain event with number {domainEvent.Number} applied");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"{_next.GetType().Name} didn't handle exception during applying domain event with number {domainEvent.Number}", ex);
                throw;
            }
        }
    }
}