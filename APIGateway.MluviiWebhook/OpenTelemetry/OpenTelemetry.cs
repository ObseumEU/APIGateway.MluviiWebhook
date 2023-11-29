using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace APIGateway.MluviiWebhook
{
    public static class OpenTelemetry
    {
        public static readonly ActivitySource Source = new("MluviiWebhook");

        public static void AddConsoleOpenTelemetry(this IServiceCollection services, IConfigurationSection configSection)
        {
            var openTelemetryOptions = configSection.Get<OpenTelemetryOptions>();
            if (openTelemetryOptions == null || string.IsNullOrEmpty(openTelemetryOptions?.UrlGrpc))
                return;

            services.Configure<OpenTelemetryOptions>(configSection);
            Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
            .AddService(openTelemetryOptions.SourceName))
           .AddSource(openTelemetryOptions.SourceName)
           .AddAspNetCoreInstrumentation()
           .AddHttpClientInstrumentation()
           .AddConsoleExporter()
           .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(openTelemetryOptions.UrlGrpc);
                options.Protocol = OtlpExportProtocol.Grpc;
            })
           .Build();
        }
    }
}
