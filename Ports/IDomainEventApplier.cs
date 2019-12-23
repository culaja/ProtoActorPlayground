using System;
using System.Threading.Tasks;
using Domain;

namespace Ports
{
    public interface IDomainEventApplier : IDisposable
    {
        Task<long> ReadLastDispatchedDomainEvent();
        
        void Pass(DomainEvent @event);
    }
}