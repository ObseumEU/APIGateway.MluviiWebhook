using APIGateway.MluviiWebhook.Contracts;
using Newtonsoft.Json.Linq;
using Silverback.Messaging.Publishing;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class KafkaPublisher : IMessagePublisher
    {
        private readonly IPublisher _messageBroker;

        public KafkaPublisher(IPublisher messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public async Task PublishAsync(JObject jobj)
        {
            var payload = new WebhookEvent
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };

            await _messageBroker.PublishAsync(payload);
        }
    }
}
