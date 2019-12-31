using System;
using Ports;
using Serilog;

namespace WorkerService
{
    internal sealed class Logger : IInternalLogger
    {
        private Logger()
        {
        }
        
        public static IInternalLogger New() => new Logger();
        
        public void Verbose(string message)
        {
            Log.Logger.Verbose(message);
        }

        public void Debug(string message)
        {
            Log.Logger.Debug(message);
        }

        public void Information(string message)
        {
            Log.Logger.Information(message);
        }

        public void Warning(string message)
        {
            Log.Logger.Warning(message);
        }

        public void Error(string message, Exception ex)
        {
            Log.Logger.Error(ex, message);
        }

        public void Fatal(string message, Exception ex)
        {
            Log.Logger.Fatal(ex, message);
        }
    }
}