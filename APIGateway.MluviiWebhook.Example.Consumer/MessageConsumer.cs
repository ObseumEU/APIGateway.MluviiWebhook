using MassTransit;
using Newtonsoft.Json;

namespace APIGateway.MluviiWebhook.Example.Consumer
{
    public class MessageConsumer : IConsumer<Contracts.WebhookEvent>
    {
        public async Task Consume(ConsumeContext<Contracts.WebhookEvent> context)
        {
            Console.WriteLine($"Receive: {JsonConvert.SerializeObject(context)}");
        }
    }
}
