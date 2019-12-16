using System.Threading.Tasks;
using Domain;

namespace Ports
{
    public interface IDomainEventApplier
    {
        Task<long> ReadLastDispatchedDomainEvent();
        
        void Pass(DomainEvent @event);
    }
}