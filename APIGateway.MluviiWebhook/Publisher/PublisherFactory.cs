using MassTransit;
using Microsoft.FeatureManagement;

namespace APIGateway.MluviiWebhook.Publisher
{
    public class PublisherFactory : IPublisherFactory
    {
        private readonly IFeatureManager _featureManager;
        private readonly IServiceProvider _serviceProvider;

        public PublisherFactory(IFeatureManager featureManager, IServiceProvider serviceProvider)
        {
            _featureManager = featureManager;
            _serviceProvider = serviceProvider;
        }

        public async Task<IMessagePublisher> GetPublisher()
        {
            if (await _featureManager.IsEnabledAsync(FeatureFlags.RABBITMQ))
            {
                // Resolve RabbitMQPublisher
                return _serviceProvider.GetRequiredService<RabbitMQPublisher>();
            }
            else
            {
                // Resolve KafkaPublisher
                return _serviceProvider.GetRequiredService<KafkaPublisher>();
            }
        }
    }

}
