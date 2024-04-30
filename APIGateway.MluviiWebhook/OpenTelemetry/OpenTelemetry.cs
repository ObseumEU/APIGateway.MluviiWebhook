using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace APIGateway.MluviiWebhook
{
    public static class OpenTelemetryCustom
    {
        public static readonly ActivitySource Source = new("MluviiWebhook");

        public static void AddConsoleOpenTelemetry(this IServiceCollection services, IConfigurationSection configSection)
        {
            var openTelemetryOptions = configSection.Get<OpenTelemetryOptions>();
            if (openTelemetryOptions == null || string.IsNullOrEmpty(openTelemetryOptions?.UrlGrpc))
            {
                return;
            }

            services.Configure<OpenTelemetryOptions>(configSection);
            services.AddOpenTelemetry()
                .ConfigureResource(a => ResourceBuilder.CreateDefault()
                    .AddService(openTelemetryOptions.SourceName))
                .WithTracing(b => b
                    .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(openTelemetryOptions.UrlGrpc);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
                )
                 .WithMetrics(b => b
                    .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(openTelemetryOptions.UrlGrpc);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })

                 ); ;
        }
    }
}
