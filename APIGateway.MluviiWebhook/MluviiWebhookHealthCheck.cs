using APIGateway.Core.Kafka;
using APIGateway.Core.MluviiClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace APIGateway.MluviiWebhook;

public class MluviiWebhookHealthCheck : IHealthCheck
{
    private readonly ILogger<MluviiWebhookHealthCheck> _log;

    private readonly IServiceScopeFactory _provide;

    public MluviiWebhookHealthCheck(IServiceScopeFactory provide, ILogger<MluviiWebhookHealthCheck> log)
    {
        _provide = provide;
        _log = log;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context)
    {
        return await CheckHealthAsync(context, default);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        using (var scope = _provide.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<IOptionsMonitor<WebhookOptions>>();
            var kafkaOptions = scope.ServiceProvider.GetService<IOptions<KafkaProduceOption>>();

            if (string.IsNullOrEmpty(kafkaOptions.Value.Topic))
            {
                return Unhealthy(
                    "Kafka topic cannot be null. Please add to appsettings KafkaProduceOption.Topic: \"some-topic\"");
            }

            if (options.CurrentValue.AutoRegister)
            {
                var mluviiClient = scope.ServiceProvider.GetService<MluviiClient>();

                if (mluviiClient == null)
                {
                    return Unhealthy("Missing mluvii client. Cannot register webhooks.");
                }

                var res = await mluviiClient.GetWebhooks();

                if (!res.response.IsSuccessful)
                {
                    return Unhealthy("Error comunicate with mluvii client.");
                }
            }

            if (string.IsNullOrEmpty(options.CurrentValue.Secret))
            {
                return Unhealthy("Missing secret for webhook.");
            }


            return HealthCheckResult.Healthy("Webhook healthy.");
        }
    }

    private HealthCheckResult Unhealthy(string msg)
    {
        _log.LogError(msg);
        return HealthCheckResult.Unhealthy(msg);
    }
}