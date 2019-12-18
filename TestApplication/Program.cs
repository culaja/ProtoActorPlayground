using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using ProtoActorAdapter;

namespace TestApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var eventsToSend = new List<DomainEvent>
            {
                DomainEvent.Of(1, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(2, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(3, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(4, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(5, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(6, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(7, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(8, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(9, "Aggregate1", "{}", "{}"),
                DomainEvent.Of(10, "Aggregate1", "{}", "{}")
            };

            var domainEventApplier = await ProtoActorDomainEventApplierBuilder.BuildUsing(new EventStoreConfiguration(
                    "localhost",
                    1113,
                    "admin",
                    "changeit",
                    "test"),
                new Uri("http://93.87.10.154:49448/DomainEventApplier"));

            var lastDispatchedDomainEvent = await domainEventApplier.ReadLastDispatchedDomainEvent();
            foreach (var domainEvent in eventsToSend.Where(e => e.Number > lastDispatchedDomainEvent))
            {
                domainEventApplier.Pass(domainEvent);
            }
            
            Console.ReadLine();
        }
    }
}