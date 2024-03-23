using Metrics.RemoteMetrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http;

namespace Metrics.Central
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<MetricsService>();
                    services.AddSingleton<MetricsContext, RemoteMetricsContext>();
                    services.AddHttpClient<IHttpRemoteMetrics, HttpRemoteMetrics>().ConfigurePrimaryHttpMessageHandler(config => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });
                });
    }
}