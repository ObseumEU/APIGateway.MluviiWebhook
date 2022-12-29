using APIGateway.Core.MluviiClient;
using Quartz;

namespace APIGateway.MluviiWebhook.Jobs
{
    [DisallowConcurrentExecution]
    public class AutoRegisterWebhookJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AutoRegisterWebhookJob> _log;

        public AutoRegisterWebhookJob(IServiceProvider provider, ILogger<AutoRegisterWebhookJob> log)
        {
            _provider = provider;
            _log = log;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                using (var scope = _provider.CreateScope())
                {
                    var registration = scope.ServiceProvider.GetService<WebhookRegistrator>();
                    await registration.RegisterWebhooks();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Cannot register webhooks.");
            }
        }
    }
}
