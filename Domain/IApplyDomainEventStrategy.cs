using System.Threading.Tasks;

namespace Domain
{
    public interface IApplyDomainEventStrategy
    {
        Task<bool> TryApply(IDomainEvent domainEvent);
    }
}