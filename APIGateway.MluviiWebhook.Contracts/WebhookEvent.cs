namespace APIGateway.MluviiWebhook.Contracts
{
    public class WebhookEvent
    {
        public string EventType { get; set; }
        public string JsonData { get; set; }
    }
}
