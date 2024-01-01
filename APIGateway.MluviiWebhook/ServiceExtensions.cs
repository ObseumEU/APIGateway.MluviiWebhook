using APIGateway.Core;
using APIGateway.Core.Cache;
using APIGateway.Core.Kafka;
using APIGateway.Core.MluviiClient;
using APIGateway.MluviiWebhook;
using APIGateway.MluviiWebhook.Jobs;
using APIGateway.MluviiWebhook.Publisher;
using MassTransit;
using Microsoft.FeatureManagement;
using Silverback.Messaging.Publishing;
using Silverback.Samples.Kafka.Batch.Producer;

namespace APIGateway.MluviiWebhook
{
    public static class ServiceExtensions
    {
        public static void AddLogging(this IServiceCollection services, IConfiguration config)
        {
            services.AddLogging(builder =>
                builder
                    .AddDebug()
                    .AddConsole()
                    .AddConfiguration(config.GetSection("Logging"))
                    .SetMinimumLevel(LogLevel.Information));
        }

        public static async Task ConfigureTelemetry(this IServiceCollection services, IConfiguration config)
        {
            var featureManager = services.BuildServiceProvider().GetService<IFeatureManager>();
            if (await featureManager.IsEnabledAsync(FeatureFlags.OPEN_TELEMETRY))
            {
                services.AddConsoleOpenTelemetry(config.GetSection("OpenTelemetryOptions"));
            }
        }

        public static async Task ConfigureRabbitMQ(this IServiceCollection services)
        {
            var featureManager = services.BuildServiceProvider().GetService<IFeatureManager>();
            if (await featureManager.IsEnabledAsync(FeatureFlags.RABBITMQ))
            {
                services.AddHttpClient();
                services.AddMassTransit(x =>
                {
                    // Configure for RabbitMQ transport
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var rabbitMQConfig = context.GetRequiredService<IConfiguration>().GetSection("RabbitMQ");
                        cfg.Host(rabbitMQConfig["Host"], rabbitMQConfig["VirtualHost"], h =>
                        {
                            h.Username(rabbitMQConfig["Username"]);
                            h.Password(rabbitMQConfig["Password"]);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });
                services.AddScoped<RabbitMQPublisher>();
            }
        }

        public static async Task ConfigureKafka(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<KafkaPublisher>();
            var featureManager = services.BuildServiceProvider().GetService<IFeatureManager>();
                //Add kafka
                services.Configure<KafkaOption>(config.GetSection("Kafka"));
                services.Configure<KafkaProduceOption>(config.GetSection("KafkaProducer"));
                services
                    .AddSilverback()
                    // Use Apache Kafka as message broker
                    .WithConnectionToMessageBroker(
                        options => options
                            .AddKafka())

                    // Delegate the inbound/outbound endpoints configuration to a separate
                    // class.
                    .AddEndpointsConfigurator<EndpointsConfigurator>();
        }

        public static void ConfigureWebhooks(this IServiceCollection services, IConfiguration config, WebApplicationBuilder builder)
        {
            services.Configure<WebhookOptions>(builder.Configuration.GetSection("Webhook"));
            services.AddSingleton<WebhookRegistrator>();
            //Add auto register webhooks
            if (config.GetSection("Webhook:AutoRegister")?.Value == "True")
            {
                services.AddQuartzJobs();
                services.AddSingletonJob<AutoRegisterWebhookJob>("*/59 * * * * ?"); //Every minute try register webhook again
            }
        }

        public static void ConfigureMluviiClient(this IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection("Mluvii");
            if (section.Exists())
            {
                services.Configure<MluviiCredentialOptions>(section);
                services.AddSingleton<ICacheService, InMemoryCache>();
                services.AddSingleton<ITokenEndpoint, TokenEndpoint>();
                services.AddSingleton<MluviiClient>();
                services.AddSingleton<IMluviiClient>(x => x.GetService<MluviiClient>());
                services.AddSingleton<IMluviiUserClient>(x => x.GetService<MluviiClient>());
            }
        }
    }
}
