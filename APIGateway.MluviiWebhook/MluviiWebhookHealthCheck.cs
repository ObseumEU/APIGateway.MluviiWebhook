using APIGateway.Core.MluviiClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace APIGateway.MluviiWebhook
{
    public class MluviiWebhookHealthCheck : IHealthCheck
    {
        public MluviiWebhookHealthCheck(IServiceScopeFactory provide, ILogger<MluviiWebhookHealthCheck> log)
        {
            _provide = provide;
            _log = log;
        }

        private readonly IServiceScopeFactory _provide;
        private readonly ILogger<MluviiWebhookHealthCheck> _log;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            using (var scope = _provide.CreateScope())
            {
                var options = scope.ServiceProvider.GetService<IOptionsMonitor<WebhookOptions>>();
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
}
