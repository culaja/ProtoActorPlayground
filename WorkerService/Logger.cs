using System;
using Microsoft.Extensions.Logging;
using Ports;

namespace WorkerService
{
    internal sealed class Logger : IInternalLogger
    {
        private readonly ILogger<Worker> _logger;

        private Logger(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        
        public static IInternalLogger NewUsing(ILogger<Worker> logger) => new Logger(logger);
        
        public void Verbose(string message)
        {
            _logger.LogTrace(message);
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Information(string message)
        {
            _logger.LogInformation(message);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);
        }

        public void Error(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }
    }
}