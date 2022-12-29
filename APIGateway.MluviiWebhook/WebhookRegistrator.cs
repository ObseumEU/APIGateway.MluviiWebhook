using APIGateway.Core.MluviiClient;
using Microsoft.Extensions.Options;
using mluvii.ApiModels.Webhooks;

namespace APIGateway.MluviiWebhook
{
    public class WebhookRegistrator
    {
        private readonly IOptions<WebhookOptions> _options;
        private readonly IMluviiClient _mluvii;
        private bool _secured;
        private ILogger<WebhookRegistrator> _log;

        public WebhookRegistrator(IOptions<WebhookOptions> options, IMluviiClient mluvii, ILogger<WebhookRegistrator> log)
        {
            _options = options;
            _mluvii = mluvii;
            _log = log;
            _secured = !string.IsNullOrEmpty(_options.Value.Secret);
        }

        public ILogger<WebhookRegistrator> Log { get; }

        public async Task RegisterWebhooks()
        {
            _log.LogInformation("Try register webhook.");
            var baseUrl = _options.Value.WebhookUrl;
            var secretUrl = baseUrl + $"?secret={_options.Value.Secret}";
            var targetUrl = _secured ? secretUrl : baseUrl;

            var res = await _mluvii.GetWebhooks();
            var currentWebhook = res.value.FirstOrDefault(w => 
                w.CallbackUrl == baseUrl ||  //Find urls without secret and with secret.
                w.CallbackUrl == secretUrl);

            var newMethods = _options.Value.Methods.ToList();
            //Not existing webhook
            if (currentWebhook != null)
            {
                await _mluvii.UpdateWebhook(currentWebhook.ID.Value, targetUrl, newMethods);
            }
            else
            {
                //Already existing webhook
                await _mluvii.AddWebhook(targetUrl, newMethods);
            }
        }
    }
}
