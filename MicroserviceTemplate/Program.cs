using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters;
using App.Metrics.Formatters.Prometheus;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace $safeprojectname$
{
    public class Program
    {
        public static IMetricsRoot Metrics { get; set; }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Metrics = AppMetrics.CreateDefaultBuilder()
                    //.OutputMetrics.AsPrometheusProtobuf()
                    .OutputMetrics.AsPrometheusPlainText()
                    .Build();

            return Host.CreateDefaultBuilder(args)
                .UseMetrics(options => {
                    options.EndpointOptions = endpointsOptions =>
                    {
                        endpointsOptions.MetricsEndpointOutputFormatter = Metrics.OutputMetricsFormatters.GetType<MetricsPrometheusTextOutputFormatter>();
                        //endpointsOptions.MetricsTextEndpointOutputFormatter = Metrics.OutputMetricsFormatters.GetType<MetricsPrometheusTextOutputFormatter>();
                        //endpointsOptions.MetricsEndpointOutputFormatter = Metrics.OutputMetricsFormatters.GetType<MetricsPrometheusProtobufOutputFormatter>();
                    };
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
