using System;
using Domain;
using Framework;
using Ports;

namespace HttpClientAdapter
{
    public sealed class HttpApplyDomainEventStrategyBuilder
    {
        private Optional<Uri> _optionalDestinationUri;
        private ILogger _logger = new NoLogger();
        
        public static HttpApplyDomainEventStrategyBuilder New() => new HttpApplyDomainEventStrategyBuilder();
        
        public HttpApplyDomainEventStrategyBuilder WithDestinationUri(Uri destinationUri)
        {
            _optionalDestinationUri = destinationUri;
            return this;
        }

        public HttpApplyDomainEventStrategyBuilder DecorateWith(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IApplyDomainEventStrategy Build()
        {
            if (_optionalDestinationUri.HasNoValue) throw new ArgumentException("Argument is not set.", nameof(_optionalDestinationUri));
            return new ApplyDomainEventStrategyLoggingDecorator(
                _logger,
                new HttpApplyDomainEventStrategy(_optionalDestinationUri.Value));
        }
    }
}