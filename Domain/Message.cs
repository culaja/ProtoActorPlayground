using System.Text;

namespace Domain
{
    public sealed class Message
    {
        public Message(string aggregateId, string data, string metaData)
        {
            AggregateId = aggregateId;
            Data = data;
            MetaData = metaData;
        }

        public static Message Of(
            string aggregateId,
            byte[] data, 
            byte[] metaData) 
                => new Message(
                    aggregateId, 
                    Encoding.UTF8.GetString(data),
                    Encoding.UTF8.GetString(metaData));

        public string AggregateId { get; }
        public string Data { get; }
        public string MetaData { get; }

        public override string ToString()
        {
            return $"{nameof(AggregateId)}: {AggregateId}, {nameof(Data)}: {Data}, {nameof(MetaData)}: {MetaData}";
        }
    }
}