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
                string topicName,
                long topicVersion,
                string data,
                string metaData,
                IApplyDomainEventStrategy applyDomainEventStrategy)
            {
                _applyDomainEventStrategy = applyDomainEventStrategy;
                Number = number;
                TopicName = topicName;
                TopicVersion = topicVersion;
                Data = data;
                MetaData = metaData;
            }

            public long Number { get; }
            public string TopicName { get; }
            public long TopicVersion { get; }
            public string Data { get; }
            public string MetaData { get; }

            public Task<Result> TryApply() => _applyDomainEventStrategy.TryApply(this);
            
            public string ToJson() => $"{{\"Number\": {Number}, \"TopicName\": \"{TopicName}\", \"TopicVersion\": {TopicVersion}, \"Data\": {Data}, \"MetaData\": {MetaData}}}";

            public override string ToString()
            {
                return $"{nameof(Number)}: {Number}, {nameof(TopicName)}: {TopicName}, {nameof(TopicVersion)}: {TopicVersion}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Number;
                yield return TopicName;
                yield return TopicVersion;
                yield return Data;
                yield return MetaData;
            }
        }
    }
}