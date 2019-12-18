using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain;
using static Newtonsoft.Json.JsonConvert;

namespace ProtoActorAdapter
{
    internal sealed class HttpDomainEventDispatcher
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        
        private readonly Uri _destinationUri;

        public HttpDomainEventDispatcher(Uri destinationUri)
        {
            _destinationUri = destinationUri;
        }
        
        public async Task<bool> Dispatch(DomainEvent @event)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _destinationUri)
            {
                Content = new StringContent(SerializeObject(@event), Encoding.UTF8, "application/json")
            };
        
            var response = await HttpClient.SendAsync(httpRequestMessage);
        
            return response.IsSuccessStatusCode;
        }
    }
}