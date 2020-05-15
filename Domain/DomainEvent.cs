using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using Newtonsoft.Json;

namespace Domain
{
    public sealed partial class DomainEventBuilder
    {
        private sealed class DomainEvent : ValueObject, IDomainEvent
        {
            private readonly IApplyDomainEventStrategy _applyDomainEventStrategy;

            public DomainEvent(
                DomainEventPosition position,
                string topicName,
                long topicVersion,
                string data,
                string metaData,
                IApplyDomainEventStrategy applyDomainEventStrategy)
            {
                _applyDomainEventStrategy = applyDomainEventStrategy;
                Position = position;
                TopicName = topicName;
                TopicVersion = topicVersion;
                Data = data;
                MetaData = metaData;
            }

            public DomainEventPosition Position { get; }
            public string TopicName { get; }
            public long TopicVersion { get; }
            public string Data { get; }
            public string MetaData { get; }

            public Task<Result> TryApply() => _applyDomainEventStrategy.TryApply(this);
            
            public string ToJson() => JsonConvert.SerializeObject(this);

            public override string ToString()
            {
                return $"{nameof(Position)}: {Position}, {nameof(TopicName)}: {TopicName}, {nameof(TopicVersion)}: {TopicVersion}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Position;
                yield return TopicName;
                yield return TopicVersion;
                yield return Data;
                yield return MetaData;
            }
        }
    }
}