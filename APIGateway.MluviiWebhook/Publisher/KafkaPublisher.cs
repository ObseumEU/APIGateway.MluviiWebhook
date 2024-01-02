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
            //Obsolate contract, new is in namespace APIGateway.MluviiWebhook.Contracts
            var payload = new APIGateway.MluviiWebhook.Contracts.WebhookEvent
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };

            await _messageBroker.PublishAsync(payload);
        }
    }
}
