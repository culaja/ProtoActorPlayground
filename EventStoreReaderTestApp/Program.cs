using System;
using Domain;
using EventStoreAdapter;
using Ports;
using static Domain.DomainEventPosition;

namespace EventStoreReaderTestApp
{
    internal sealed class EventStoreReceiver : IEventStoreStreamMessageReceiver
    {
        public IDomainEvent Receive(DomainEventBuilder builder)
        {
            var domainEvent = builder.Build();
            
            Console.WriteLine(builder.Build());

            return domainEvent;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var subscription = EventStoreReader
                .BuildUsing(new Uri("tcp://admin:changeit@localhost:1113"))
                .SubscribeToAllEvents(Start, new EventStoreReceiver());

            Console.ReadLine();
            subscription.Dispose();
        }
    }
}