using System.Collections.Generic;
using Framework;

namespace Domain
{
    public sealed class DomainEvent : ValueObject
    {
        public DomainEvent(
            long number,
            string aggregateId,
            string data,
            string metaData)
        {
            Number = number;
            AggregateId = aggregateId;
            Data = data;
            MetaData = metaData;
        }

        public static DomainEvent Of(
            long number,
            string aggregateId,
            string data, 
            string metaData) 
                => new DomainEvent(
                    number,
                    aggregateId, 
                    data,
                    metaData);

        public long Number { get; }
        public string AggregateId { get; }
        public string Data { get; }
        public string MetaData { get; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(AggregateId)}: {AggregateId}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return AggregateId;
            yield return Data;
            yield return MetaData;
        }
    }
}