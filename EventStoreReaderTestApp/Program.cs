using System;
using Domain;
using EventStoreAdapter;
using Ports;

namespace EventStoreReaderTestApp
{
    internal sealed class EventStoreReceiver : IEventStoreStreamMessageReceiver
    {
        public void Receive(Message message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var subscription = new EventStoreReader("tcp://localhost:1113").SubscribeTo(
                StreamName.Of("Library"), new EventStoreReceiver());

            Console.ReadLine();
            subscription.Dispose();
        }
    }
}