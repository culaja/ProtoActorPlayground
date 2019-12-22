using System;

namespace Ports
{
    public interface ILogger
    {
        void Verbose(string message);
        void Debug(string message);
        void Information(string message);
        void Warning(string message);
        void Error(string message, Exception ex);
        void Fatal(string message, Exception ex);
    }
}