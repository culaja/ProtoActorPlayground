using System;

namespace Ports
{
    public sealed class NoInternalLogger : IInternalLogger
    {
        public void Verbose(string message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Information(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Error(string message, Exception ex)
        {
        }

        public void Fatal(string message, Exception ex)
        {
        }
    }
}