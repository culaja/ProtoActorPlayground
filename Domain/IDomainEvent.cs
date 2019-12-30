using System.Threading.Tasks;
using Framework;

namespace Domain
{
    public interface IDomainEvent
    {
        long Number { get; }
        
        string TopicName { get; }

        Task<Result> TryApply();

        string ToJson();
    }
}