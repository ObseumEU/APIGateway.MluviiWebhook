using APIGateway.MluviiWebhook.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RabbitMQPublisher> _log;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint, ILogger<RabbitMQPublisher> log)
        {
            _publishEndpoint = publishEndpoint;
            _log = log;
        }

        public async Task PublishAsync(JObject jobj)
        {
            var payload = new WebhookEventContract
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };
            _log.LogInformation("Produce mesage: " + JsonConvert.SerializeObject(payload));
            await _publishEndpoint.Publish<WebhookEventContract>(payload);
        }
    }
}
