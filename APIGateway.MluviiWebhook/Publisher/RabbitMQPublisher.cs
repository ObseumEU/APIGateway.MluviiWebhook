using APIGateway.MluviiWebhook.Contracts;
using MassTransit;
using Newtonsoft.Json.Linq;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync(JObject jobj)
        {
            var payload = new WebhookEventContract
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };

            await _publishEndpoint.Publish<WebhookEventContract>(payload);
        }
    }
}
