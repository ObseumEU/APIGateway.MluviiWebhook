using MassTransit;
using Newtonsoft.Json;

namespace APIGateway.MluviiWebhook.Example.Consumer
{
    public class MessageConsumer : IConsumer<Contracts.WebhookEventContract>
    {
        public async Task Consume(ConsumeContext<Contracts.WebhookEventContract> context)
        {
            Console.WriteLine($"Receive: {JsonConvert.SerializeObject(context)}");
        }
    }
}
