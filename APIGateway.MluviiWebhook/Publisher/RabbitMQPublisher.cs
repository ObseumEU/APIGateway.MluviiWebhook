using APIGateway.MluviiWebhook.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IOptions<RabbitMQOptions> _options;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint, IOptions<RabbitMQOptions> options)
        {
            _publishEndpoint = publishEndpoint;
            _options = options;
        }

        public async Task PublishAsync(JObject jobj)
        {
            var payload = new WebhookEvent
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };

            await _publishEndpoint.Publish<WebhookEvent>(payload, ctx =>
            {
                if (!string.IsNullOrEmpty(_options.Value?.Topic))
                {
                    ctx.SetRoutingKey(_options.Value.Topic);
                }
            });
        }
    }
}
