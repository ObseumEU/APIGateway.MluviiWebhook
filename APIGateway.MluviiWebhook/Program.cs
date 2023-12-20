using APIGateway.Core;
using APIGateway.Core.Cache;
using APIGateway.Core.Kafka;
using APIGateway.Core.MluviiClient;
using APIGateway.MluviiWebhook;
using APIGateway.MluviiWebhook.Jobs;
using Microsoft.FeatureManagement;
using Sentry;
using Silverback.Samples.Kafka.Batch.Producer;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var services = builder.Services;

// Add services to the container.
builder.Services.AddFeatureManagement();
services.AddLogging(builder =>
    builder
        .AddDebug()
        .AddConsole()
        .AddConfiguration(config.GetSection("Logging"))
        .SetMinimumLevel(LogLevel.Information)
);
builder.Services.AddControllers();
builder.WebHost.UseSentry();


var featureManager = services.BuildServiceProvider().GetService<IFeatureManager>();
if (await featureManager.IsEnabledAsync(FeatureFlags.OPEN_TELEMETRY))
{
    builder.Services.AddConsoleOpenTelemetry(config.GetSection("OpenTelemetryOptions"));
}

if (await featureManager.IsEnabledAsync(FeatureFlags.RABBITMQ))
{

}

    //Options 
    builder.Services.Configure<WebhookOptions>(builder.Configuration.GetSection("Webhook"));

//Add kafka
builder.Services.Configure<KafkaOption>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<KafkaProduceOption>(builder.Configuration.GetSection("KafkaProducer"));
services
    .AddSilverback()
    // Use Apache Kafka as message broker
    .WithConnectionToMessageBroker(
        options => options
            .AddKafka())

    // Delegate the inbound/outbound endpoints configuration to a separate
    // class.
    .AddEndpointsConfigurator<EndpointsConfigurator>();

builder.Services.AddSingleton<WebhookRegistrator>();

//Add auto register webhooks
if (config.GetSection("Webhook:AutoRegister")?.Value == "True")
{
    builder.Services.AddQuartzJobs();
    services.AddSingletonJob<AutoRegisterWebhookJob>("*/59 * * * * ?"); //Every minute try register webhook again
}

//Add mluvii client
var section = builder.Configuration.GetSection("Mluvii");
if (section.Exists())
{
    builder.Services.Configure<MluviiCredentialOptions>(section);
    builder.Services.AddSingleton<ICacheService, InMemoryCache>();
    builder.Services.AddSingleton<ITokenEndpoint, TokenEndpoint>();
    builder.Services.AddSingleton<MluviiClient>();
    builder.Services.AddSingleton<IMluviiClient>(x => x.GetService<MluviiClient>());
    builder.Services.AddSingleton<IMluviiUserClient>(x => x.GetService<MluviiClient>());
}

builder.Services.AddSingleton<MluviiWebhookHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<MluviiWebhookHealthCheck>("Webhook");

var app = builder.Build();

if (config.GetSection("Sentry").Exists())
{
    app.UseSentryTracing();
    SentrySdk.CaptureMessage("Webhook service started!");
}

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");


app.Run();

public partial class Program
{
}