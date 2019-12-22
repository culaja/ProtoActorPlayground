using System;
using Ports;
using Proto;

namespace ProtoActorAdapter.Logging
{
    internal sealed class ActorLoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly string _actorId;

        private ActorLoggingMiddleware(ILogger logger, string actorId)
        {
            _logger = logger;
            _actorId = actorId;
        }
        
        public static ActorLoggingMiddleware For(ILogger logger, string actorId) =>
            new ActorLoggingMiddleware(logger, actorId);

        public Receiver ReceiveHook(Receiver next) => async (context, envelope) =>
        {
            try
            {
                _logger.Debug($"{_actorId} received message from {envelope.Sender.Name()} with content: '{envelope.Message}'.");
                await next(context, envelope);
                _logger.Verbose($"{_actorId} finished processing of message received from {envelope.Sender.Name()} with content: '{envelope.Message}'.");
            }
            catch (Exception ex)
            {
                _logger.Error($"{_actorId} didn't handle exception during processing of message received from {envelope.Sender.Name()} with content {envelope.Message}", ex);
                throw;
            }
        };

        public Sender SendHook(Sender next) => async (context, target, envelope) =>
        {
            try
            {
                _logger.Debug($"{_actorId} sending a message to {target.Name()} with content: '{envelope.Message}'.");
                await next(context, target, envelope);
                _logger.Verbose($"{_actorId} has sent the message to {target.Name()} with content: '{envelope.Message}'.");
            }
            catch (Exception ex)
            {
                _logger.Error($"{_actorId} didn't handle exception during sending of message to {target.Name()} with content {envelope.Message}", ex);
                throw;
            }
        };
    }
}