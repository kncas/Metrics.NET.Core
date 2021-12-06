using Metrics.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics.RemoteMetrics
{
    public interface IHttpRemoteMetrics
    {
        Task<JsonMetricsContext> FetchRemoteMetrics(Uri remoteUri, Func<string, JsonMetricsContext> deserializer, CancellationToken token);
    }
}