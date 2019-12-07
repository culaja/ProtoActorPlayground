using System.Text;

namespace Domain
{
    public sealed class DomainEvent
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
            byte[] data, 
            byte[] metaData) 
                => new DomainEvent(
                    number,
                    aggregateId, 
                    Encoding.UTF8.GetString(data),
                    Encoding.UTF8.GetString(metaData));

        public long Number { get; }
        public string AggregateId { get; }
        public string Data { get; }
        public string MetaData { get; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(AggregateId)}: {AggregateId}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
        }
    }
}