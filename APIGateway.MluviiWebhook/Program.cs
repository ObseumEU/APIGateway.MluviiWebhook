using APIGateway.MluviiWebhook;
using Microsoft.FeatureManagement;
using Sentry;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
ConfigurePipeline(app);
app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    var services = builder.Services;
    var config = builder.Configuration;
    builder.WebHost.UseSentry();
    services.AddFeatureManagement();
    services.AddLogging(config);
    services.AddControllers();
    services.ConfigureTelemetry(config);
    services.ConfigureMluviiClient(config);
    services.ConfigureRabbitMQ(config);
    services.ConfigureKafka(config);
    services.ConfigureWebhooks(config, builder);
    services.AddHealthChecks().AddCheck<MluviiWebhookHealthCheck>("Webhook");
    builder.Services.AddSingleton<MluviiWebhookHealthCheck>();
}

void ConfigurePipeline(WebApplication app)
{
    var config = app.Configuration;

    if (config.GetSection("Sentry").Exists())
    {
        app.UseSentryTracing();
        SentrySdk.CaptureMessage("Webhook service started!");
    }

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");
}

public partial class Program
{
}