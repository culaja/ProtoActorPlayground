using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;

namespace Domain
{
    public sealed partial class DomainEventBuilder
    {
        private sealed class DomainEvent : ValueObject, IDomainEvent
        {
            private readonly IApplyDomainEventStrategy _applyDomainEventStrategy;

            public DomainEvent(
                long number,
                string aggregateId,
                long aggregateVersion,
                string data,
                string metaData,
                IApplyDomainEventStrategy applyDomainEventStrategy)
            {
                _applyDomainEventStrategy = applyDomainEventStrategy;
                Number = number;
                AggregateId = aggregateId;
                AggregateVersion = aggregateVersion;
                Data = data;
                MetaData = metaData;
            }

            public long Number { get; }
            public string AggregateId { get; }
            public long AggregateVersion { get; }
            public string Data { get; }
            public string MetaData { get; }

            public Task<bool> TryApply() => _applyDomainEventStrategy.TryApply(this);
            public string ToJson() => $"{{\"Number\" = {Number},\"AggregateId\" =\"{AggregateId}\",{{\"AggregateVersion\" = {AggregateVersion},Data = \"{Data}\",MetaData =\"{MetaData}\"}}";

            public override string ToString()
            {
                return $"{nameof(Number)}: {Number}, {nameof(AggregateId)}: {AggregateId}, {nameof(AggregateVersion)}: {AggregateVersion}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Number;
                yield return AggregateId;
                yield return AggregateVersion;
                yield return Data;
                yield return MetaData;
            }
        }
    }
    
    
}