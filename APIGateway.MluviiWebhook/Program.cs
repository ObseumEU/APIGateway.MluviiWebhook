using APIGateway.Core;
using APIGateway.Core.Cache;
using APIGateway.Core.Kafka;
using APIGateway.Core.MluviiClient;
using APIGateway.MluviiWebhook;
using Microsoft.FeatureManagement;
using Silverback.Samples.Kafka.Batch.Producer;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
builder.Services.AddFeatureManagement();
builder.Services.AddControllers();

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
// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");


//Register webhooks
using (var serviceScope = app.Services.CreateScope())
{
    var svr = serviceScope.ServiceProvider;
    var webhookRegistrator = svr.GetService<WebhookRegistrator>();
    webhookRegistrator.RegisterWebhooks().Wait();
}

app.Run();


public partial class Program { }
