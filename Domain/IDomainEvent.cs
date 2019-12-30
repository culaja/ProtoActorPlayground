using System.Threading.Tasks;

namespace Domain
{
    public interface IDomainEvent
    {
        long Number { get; }
        
        string TopicName { get; }

        Task<bool> TryApply();

        string ToJson();
    }
}