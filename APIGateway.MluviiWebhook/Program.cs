using APIGateway.MluviiWebhook;
using Microsoft.FeatureManagement;
using Sentry;

var builder = WebApplication.CreateBuilder(args);
await ConfigureServices(builder);
var app = builder.Build();
ConfigurePipeline(app);
app.Run();

async Task ConfigureServices(WebApplicationBuilder builder)
{
    var services = builder.Services;
    var config = builder.Configuration;
    builder.WebHost.UseSentry();
    services.AddFeatureManagement();
    services.AddLogging(config);
    services.AddControllers();
    await services.ConfigureTelemetry(config);
    services.ConfigureMluviiClient(config);
    await services.ConfigureRabbitMQ(config);
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