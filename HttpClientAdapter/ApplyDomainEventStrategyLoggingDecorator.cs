using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Domain;
using Ports;

namespace HttpClientAdapter
{
    internal sealed class ApplyDomainEventStrategyLoggingDecorator : IApplyDomainEventStrategy
    {
        private readonly IInternalLogger _internalLogger;
        private readonly IApplyDomainEventStrategy _next;

        public ApplyDomainEventStrategyLoggingDecorator(IInternalLogger internalLogger, IApplyDomainEventStrategy next)
        {
            _internalLogger = internalLogger;
            _next = next;
        }

        public async Task<bool> TryApply(IDomainEvent domainEvent)
        {
            try
            {
                _internalLogger.Verbose($"Applying domain event with number {domainEvent.Number} ...");
                var result = await _next.TryApply(domainEvent);
                _internalLogger.Verbose($"Domain event with number {domainEvent.Number} applied");
                return result;
            }
            catch (Exception ex)
            {
                _internalLogger.Error($"{_next.GetType().Name} didn't handle exception during applying domain event with number {domainEvent.Number}", ex);
                throw;
            }
        }
    }
}