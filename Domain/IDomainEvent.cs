using System.Threading.Tasks;
using Framework;

namespace Domain
{
    public interface IDomainEvent
    {
        DomainEventPosition Position { get; }
        
        string TopicName { get; }

        Task<Result> TryApply();

        string ToJson();
    }
}