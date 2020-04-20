using System;
using Domain;
using Framework;
using Ports;

namespace HttpClientAdapter
{
    public sealed class HttpApplyDomainEventStrategyBuilder
    {
        private Optional<Uri> _optionalDestinationUri;
        private int _maxConnectionsPerServer = 100;
        private IInternalLogger _internalLogger = new NoInternalLogger();
        
        public static HttpApplyDomainEventStrategyBuilder New() => new HttpApplyDomainEventStrategyBuilder();
        
        public HttpApplyDomainEventStrategyBuilder WithDestinationUri(Uri destinationUri)
        {
            _optionalDestinationUri = destinationUri;
            return this;
        }

        public HttpApplyDomainEventStrategyBuilder WithMaxConnectionsPerServer(int maxConnectionsPerServer)
        {
            _maxConnectionsPerServer = maxConnectionsPerServer;
            return this;
        }

        public HttpApplyDomainEventStrategyBuilder DecorateWith(IInternalLogger internalLogger)
        {
            _internalLogger = internalLogger;
            return this;
        }

        public IApplyDomainEventStrategy Build()
        {
            if (_optionalDestinationUri.HasNoValue) throw new ArgumentException("Argument is not set.", nameof(_optionalDestinationUri));
            return new ApplyDomainEventStrategyLoggingDecorator(
                _internalLogger,
                new HttpApplyDomainEventStrategy(
                    _optionalDestinationUri.Value,
                    _maxConnectionsPerServer));
        }
    }
}