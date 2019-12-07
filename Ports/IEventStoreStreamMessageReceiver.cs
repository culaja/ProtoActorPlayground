using Domain;

namespace Ports
{
    public interface IEventStoreStreamMessageReceiver
    {
        void Receive(Message message);
    }
}