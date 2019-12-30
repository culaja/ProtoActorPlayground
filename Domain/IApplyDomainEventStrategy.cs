using System.Threading.Tasks;
using Framework;

namespace Domain
{
    public interface IApplyDomainEventStrategy
    {
        Task<Result> TryApply(IDomainEvent domainEvent);
    }
}