namespace APIGateway.MluviiWebhook;

public class WebhookOptions
{
    public string Secret { get; set; }
    public bool AutoRegister { get; set; }
    public string[] Methods { get; set; }
    public string WebhookUrl { get; set; }
}