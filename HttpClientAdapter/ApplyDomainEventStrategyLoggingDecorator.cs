using System;
using System.Threading.Tasks;
using Domain;
using Framework;
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

        public async Task<Result> TryApply(IDomainEvent domainEvent)
        {
            try
            {
                _internalLogger.Verbose($"Applying domain event with number {domainEvent.Position} ...");
                var result = await _next.TryApply(domainEvent);
                LogResponse(domainEvent, result);
                
                return result;
            }
            catch (Exception ex)
            {
                _internalLogger.Error($"{_next.GetType().Name} didn't handle exception during applying domain event with number {domainEvent.Position}", ex);
                throw;
            }
        }

        private void LogResponse(IDomainEvent domainEvent, Result result)
        {
            if (result.IsSuccess)
            {
                _internalLogger.Verbose($"Domain event with number {domainEvent.Position} applied.");
            }
            else
            {
                _internalLogger.Warning($"Failed to apply domain event '{domainEvent}'. Reason: {result.Error}");
            }
        }
    }
}