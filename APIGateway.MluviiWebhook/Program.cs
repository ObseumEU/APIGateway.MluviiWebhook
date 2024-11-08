using APIGateway.MluviiWebhook;
using APIGateway.MluviiWebhook.Publisher;
using Microsoft.FeatureManagement;
using Obseum.Telemetry;
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
    builder.AddObservability();
    services.AddFeatureManagement();
    services.AddLogging(config);
    services.AddControllers();
    services.ConfigureMluviiClient(config);
    await services.ConfigureRabbitMQ();
    await services.ConfigureKafka(config);
    services.AddScoped<IPublisherFactory, PublisherFactory>();


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