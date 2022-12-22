using APIGateway.Core;
using APIGateway.Core.Cache;
using APIGateway.Core.Kafka;
using APIGateway.Core.MluviiClient;
using APIGateway.MluviiWebhook;
using Microsoft.FeatureManagement;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFeatureManagement();
builder.Services.AddControllers();

//Options 
builder.Services.Configure<WebhookOptions>(builder.Configuration.GetSection("Webhook"));

//Add kafka
builder.Services.Configure<KafkaOption>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<KafkaProduceOption>(builder.Configuration.GetSection("KafkaProducer"));
builder.Services.AddScoped<IMessageBroker, KafkaClient>();

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
#if !DEBUG
    var svr = serviceScope.ServiceProvider;
    var webhookRegistrator = svr.GetService<WebhookRegistrator>();
    webhookRegistrator.RegisterWebhooks().Wait();
#endif

}

app.Run();


public partial class Program { }
