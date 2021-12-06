using Metrics.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics.RemoteMetrics
{
    public class HttpRemoteMetrics : IHttpRemoteMetrics
    {
        private readonly HttpClient httpClient;

        public HttpRemoteMetrics(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            this.httpClient = httpClient;
        }

        //private class CustomClient : WebClients
        //{
        //    protected override WebRequest GetWebRequest(Uri address)
        //    {
        //        HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
        //        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        //        return request;
        //    }
        //}

        public async Task<JsonMetricsContext> FetchRemoteMetrics(Uri remoteUri, Func<string, JsonMetricsContext> deserializer, CancellationToken token)
        {
            var result = await httpClient.GetAsync(remoteUri);
            var stringResult = await result.Content.ReadAsStringAsync();
            return deserializer(stringResult);
        }
    }
}