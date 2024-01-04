using Newtonsoft.Json.Linq;
using Silverback.Messaging.Publishing;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class KafkaPublisher : IMessagePublisher
    {
        private readonly IPublisher _messageBroker;
        private readonly ILogger<KafkaPublisher> _log;

        public KafkaPublisher(IPublisher messageBroker, ILogger<KafkaPublisher> log)
        {
            _messageBroker = messageBroker;
            _log = log;
        }

        public async Task PublishAsync(JObject jobj)
        {
            _log.LogInformation($"Produce to Kafka topic {jobj["eventType"].ToString()} body{jobj.ToString()}");
            //Obsolate contract, new is in namespace APIGateway.MluviiWebhook.Contracts
            var payload = new APIGateway.Core.Kafka.Messages.WebhookEvent
            {
                EventType = jobj["eventType"].ToString(),
                JsonData = jobj["data"].ToString()
            };

            await _messageBroker.PublishAsync(payload);
        }
    }
}
