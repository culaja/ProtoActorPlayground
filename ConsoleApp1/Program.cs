using System;
using EventStore.ClientAPI;
using Proto;
using Proto.Persistence.EventStore;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new RootContext();
            
            var esConnection = EventStoreConnection.Create(
                "ConnectTo=tcp://admin:changeit@localhost:1113; DefaultUserCredentials=admin:changeit;",
                ConnectionSettings.Create().KeepReconnecting(),
                "Proto.Persistence.EventStore.Sample");

            esConnection.ConnectAsync().Wait();
            
            var provider = new EventStoreProvider(esConnection);
            var props = Props.FromProducer(() => new BookActor(provider, "Library"));
            var pid = context.Spawn(props);
            context.Send(pid, new BookBorrowed("Na Drini Cuprija", "John Doe"));
            Console.ReadLine();
        }
    }
}