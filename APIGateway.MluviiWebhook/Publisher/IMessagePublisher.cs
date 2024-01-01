using Newtonsoft.Json.Linq;

namespace APIGateway.MluviiWebhook.Publisher
{
    public interface IMessagePublisher
    {
        Task PublishAsync(JObject jobj);
    }
}