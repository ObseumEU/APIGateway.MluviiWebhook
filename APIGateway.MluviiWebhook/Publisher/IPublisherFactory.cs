namespace APIGateway.MluviiWebhook.Publisher
{
    public interface IPublisherFactory
    {
        Task<IMessagePublisher> GetPublisher();
    }
}