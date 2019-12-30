using System;

namespace Ports
{
    public interface IInternalLogger
    {
        void Verbose(string message);
        void Debug(string message);
        void Information(string message);
        void Warning(string message);
        void Error(string message, Exception ex);
    }
}