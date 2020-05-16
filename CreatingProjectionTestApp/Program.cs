using System;
using System.Threading;
using Domain;
using EventStoreAdapter;

namespace CreatingProjectionTestApp
{
    class Program
    {
        static void Main()
        {
            var connectionString = new Uri("tcp://admin:changeit@localhost:1113");
            var streamPrefix = StreamPrefix.Of("Domain");
            
            var projectionCreator = SourceProjectionCreator.NewFor(connectionString, ConsoleInternalLogger.New());

            projectionCreator.Create(streamPrefix, new CancellationToken());
        }
    }
}