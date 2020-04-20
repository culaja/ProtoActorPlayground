using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientAdapter
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> ProcessAsync(
            this HttpClient httpClient,
            HttpRequestMessage httpRequestMessage)
        {
            try
            {
                return await httpClient.SendAsync(httpRequestMessage);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                };
            }
        }
        
        public static async Task<string> ReadyBodyAsString(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.Content != null)
            {
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }
    }
}