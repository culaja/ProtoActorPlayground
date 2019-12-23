using System.Threading.Tasks;

namespace Domain
{
    public interface IDomainEvent
    {
        long Number { get; }
        
        string AggregateId { get; }

        Task<bool> TryApply();

        string ToJson();
    }
}