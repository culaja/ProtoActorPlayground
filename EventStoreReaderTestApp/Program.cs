using System;
using System.Threading;
using Domain;
using EventStoreAdapter;
using Ports;

namespace EventStoreReaderTestApp
{
    internal sealed class EventStoreReceiver : IEventStoreStreamMessageReceiver
    {
        public void Receive(DomainEventBuilder builder)
        {
            Console.WriteLine(builder.Build());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var subscription = EventStoreReader.BuildUsing(new Uri("tcp://localhost:1113")).SubscribeTo(
                StreamPrefix.Of("AllDomainEvents"), 
                -1,
                new CancellationToken(),
                new EventStoreReceiver());

            Console.ReadLine();
            subscription.Dispose();
        }
    }
}