using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Framework;

namespace HttpClientAdapter
{
    internal sealed class HttpApplyDomainEventStrategy : IApplyDomainEventStrategy
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        
        private readonly Uri _destinationUri;

        public HttpApplyDomainEventStrategy(Uri destinationUri)
        {
            _destinationUri = destinationUri;
        }
        
        public async Task<Result> TryApply(IDomainEvent domainEvent)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _destinationUri)
            {
                Content = new StringContent(domainEvent.ToJson(), Encoding.UTF8, "application/json")
            };
        
            var response = await HttpClient.SendAsync(httpRequestMessage);

            var errorMessage = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? Result.Ok()
                : Result.Fail($"Code: {response.ReasonPhrase}, Error: {errorMessage}");
        }
    }
}