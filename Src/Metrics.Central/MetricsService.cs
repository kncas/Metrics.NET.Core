using Metrics.Json;
using Metrics.RemoteMetrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics.Central
{
    public class MetricsService : IHostedService
    {
        public MetricsService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private const string remotesFile = "remotes.txt";
        private readonly IServiceProvider serviceProvider;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Metric.Config
                .WithJsonDeserialzier(JsonConvert.DeserializeObject<JsonMetricsContext>)
                .WithAllCounters();

            var remotes = ReadRemotesFromConfig();

            var remoteMetrics = serviceProvider.GetRequiredService<IHttpRemoteMetrics>();
            foreach (var uri in remotes)
            {
                var remoteMetricsContext = new RemoteMetricsContext(uri,
                    TimeSpan.FromSeconds(1), JsonConvert.DeserializeObject<JsonMetricsContext>, remoteMetrics);

                Metric.Config.RegisterRemote(uri.ToString(), uri, TimeSpan.FromSeconds(1), remoteMetricsContext);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private IEnumerable<Uri> ReadRemotesFromConfig()
        {
            if (!File.Exists(remotesFile))
            {
                yield break;
            }

            var remotes = File.ReadAllLines("remotes.txt")
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => !l.StartsWith("#"));

            foreach (var remote in remotes)
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(remote, UriKind.Absolute);
                }
                catch (Exception x)
                {
                    MetricsErrorHandler.Handle(x, "Unable to read uri from remotes.txt file");
                }

                if (uri != null)
                {
                    yield return uri;
                }
            }
        }
    }
}