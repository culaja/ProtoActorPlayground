using System;
using Ports;

namespace ProtoActorAdapter.Logging
{
    public sealed class ConsoleLogger : ILogger
    {
        public static ILogger New() => new ConsoleLogger();
        
        public void Verbose(string message)
        {
            Console.WriteLine($"[{nameof(Verbose)}] {message}");
        }

        public void Debug(string message)
        {
            Console.WriteLine($"[{nameof(Debug)}] {message}");
        }

        public void Information(string message)
        {
            Console.WriteLine($"[{nameof(Information)}] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[{nameof(Warning)}] {message}");
        }

        public void Error(string message, Exception ex)
        {
            Console.WriteLine($"[{nameof(Error)}] {message}.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        public void Fatal(string message, Exception ex)
        {
            Console.WriteLine($"[{nameof(Fatal)}] {message}.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}