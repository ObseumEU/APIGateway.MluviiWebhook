namespace APIGateway.MluviiWebhook.Contracts
{
    public class WebhookEventContract
    {
        public string EventType { get; set; }
        public string JsonData { get; set; }
    }
}
