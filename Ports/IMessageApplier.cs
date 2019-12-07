using Domain;

namespace Ports
{
    public interface IMessageApplier
    {
        long LastAppliedMessage { get; }
        
        void Pass(DomainEvent message);
    }
}