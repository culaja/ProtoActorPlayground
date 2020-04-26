using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Ports
{
    public interface IDomainEventApplier : IDisposable
    {
        Task<long> ReadLastKnownDispatchedDomainEventNumber(CancellationToken cancellationToken);
        
        void Pass(IDomainEvent domainEvent);
    }
}