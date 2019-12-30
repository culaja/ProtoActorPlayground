using System;
using System.Threading.Tasks;
using Framework;

namespace Domain
{
    public sealed partial class DomainEventBuilder
    {
        private long? _optionalNumber;
        private Optional<string> _optionalAggregateId;
        private long? _optionalAggregateVersion;
        private Optional<string> _optionalData;
        private Optional<string> _optionalMetadata;
        private IApplyDomainEventStrategy _applyDomainEventStrategy = new AlwaysSuccessApplyDomainEventStrategy();
        
        public static DomainEventBuilder New() => new DomainEventBuilder();

        public DomainEventBuilder WithNumber(long number)
        {
            _optionalNumber = number;
            return this;
        }

        public DomainEventBuilder ForAggregate(string aggregateId)
        {
            _optionalAggregateId = aggregateId;
            return this;
        }

        public DomainEventBuilder WithAggregateVersion(long aggregateVersion)
        {
            _optionalAggregateVersion = aggregateVersion;
            return this;
        }

        public DomainEventBuilder WithData(string data)
        {
            _optionalData = data;
            return this;
        }

        public DomainEventBuilder WithMetadata(string metadata)
        {
            _optionalMetadata = metadata;
            return this;
        }

        public DomainEventBuilder Using(IApplyDomainEventStrategy strategy)
        {
            _applyDomainEventStrategy = strategy;
            return this;
        }

        public IDomainEvent Build()
        {
            if (!_optionalNumber.HasValue) throw new ArgumentException($"Argument not set in {nameof(DomainEventBuilder)}", nameof(_optionalNumber));
            if (_optionalAggregateId.HasNoValue) throw new ArgumentException($"Argument not set in {nameof(DomainEventBuilder)}", nameof(_optionalAggregateId));
            if (!_optionalAggregateVersion.HasValue) throw new ArgumentException($"Argument not set in {nameof(DomainEventBuilder)}", nameof(_optionalAggregateId));
            if (_optionalData.HasNoValue) throw new ArgumentException($"Argument not set in {nameof(DomainEventBuilder)}", nameof(_optionalData));
            if (_optionalMetadata.HasNoValue) throw new ArgumentException($"Argument not set in {nameof(DomainEventBuilder)}", nameof(_optionalMetadata));
            return new DomainEvent(
                _optionalNumber.Value,
                _optionalAggregateId.Value,
                _optionalAggregateVersion.Value,
                _optionalData.Value,
                _optionalMetadata.Value,
                _applyDomainEventStrategy);
        }
        
        private sealed class AlwaysSuccessApplyDomainEventStrategy : IApplyDomainEventStrategy
        {
            public Task<Result> TryApply(IDomainEvent domainEvent) => Task.FromResult(Result.Ok());
        }
    }
}