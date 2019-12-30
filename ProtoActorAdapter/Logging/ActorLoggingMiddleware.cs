using System;
using Ports;
using Proto;

namespace ProtoActorAdapter.Logging
{
    internal sealed class ActorLoggingMiddleware
    {
        private readonly IInternalLogger _internalLogger;
        private readonly string _actorId;

        private ActorLoggingMiddleware(IInternalLogger internalLogger, string actorId)
        {
            _internalLogger = internalLogger;
            _actorId = actorId;
        }
        
        public static ActorLoggingMiddleware For(IInternalLogger internalLogger, string actorId) =>
            new ActorLoggingMiddleware(internalLogger, actorId);

        public Receiver ReceiveHook(Receiver next) => async (context, envelope) =>
        {
            try
            {
                _internalLogger.Debug($"{_actorId} received message from {envelope.Sender.Name()} with content: '{envelope.Message}'.");
                await next(context, envelope);
                _internalLogger.Verbose($"{_actorId} finished processing of message received from {envelope.Sender.Name()} with content: '{envelope.Message}'.");
            }
            catch (Exception ex)
            {
                _internalLogger.Error($"{_actorId} didn't handle exception during processing of message received from {envelope.Sender.Name()} with content {envelope.Message}", ex);
                throw;
            }
        };

        public Sender SendHook(Sender next) => async (context, target, envelope) =>
        {
            try
            {
                _internalLogger.Debug($"{_actorId} sending a message to {target.Name()} with content: '{envelope.Message}'.");
                await next(context, target, envelope);
                _internalLogger.Verbose($"{_actorId} has sent the message to {target.Name()} with content: '{envelope.Message}'.");
            }
            catch (Exception ex)
            {
                _internalLogger.Error($"{_actorId} didn't handle exception during sending of message to {target.Name()} with content {envelope.Message}", ex);
                throw;
            }
        };
    }
}