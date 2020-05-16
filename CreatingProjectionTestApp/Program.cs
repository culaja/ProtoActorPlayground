using System;
using Domain;
using EventStoreAdapter;

namespace CreatingProjectionTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = new Uri("tcp://admin:changeit@localhost:1113");
            var streamPrefix = StreamPrefix.Of("Domain");
            
            var projectionCreator = SourceProjectionCreator.NewFor(connectionString);

            var isCreated = projectionCreator.CreateFor(streamPrefix);
            
            Console.WriteLine(isCreated 
                ? $"Successfully created projection for stream prefix '{streamPrefix}'"
                : $"Projection for stream prefix '{streamPrefix}' is already created.");
        }
    }
}