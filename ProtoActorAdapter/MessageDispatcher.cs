using System.Threading.Tasks;
using Domain;

namespace ProtoActorAdapter
{
    internal sealed class MessageDispatcher
    {
        public Task<bool> Dispatch(DomainEvent message)
        {
            return Task.FromResult(false);
        }
    }
}