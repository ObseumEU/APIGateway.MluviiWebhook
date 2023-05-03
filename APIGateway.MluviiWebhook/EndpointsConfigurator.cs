using APIGateway.Core.Kafka;
using APIGateway.Core.Kafka.Messages;
using APIGateway.MluviiWebhook;
using Microsoft.Extensions.Options;
using Silverback.Messaging.Configuration;

namespace Silverback.Samples.Kafka.Batch.Producer;

public class EndpointsConfigurator : IEndpointsConfigurator
{
    private readonly IOptions<KafkaOption> _kafkaOption;
    private readonly IOptions<KafkaProduceOption> _producerOption;
    private readonly IServiceProvider _provider;

    public EndpointsConfigurator(IOptions<KafkaOption> kafkaOption, IOptions<KafkaProduceOption> producerOption, IServiceProvider provider)
    {
        _kafkaOption = kafkaOption;
        _producerOption = producerOption;
        _provider = provider;
    }

    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder
            .AddKafkaEndpoints(
                endpoints => endpoints

                    // Configure the properties needed by all consumers/producers
                    .Configure(
                        config =>
                        {
                            // The bootstrap server address is needed to connect
                            config.BootstrapServers =
                                $"PLAINTEXT://{_kafkaOption.Value.Host}";
                        })

                    // Produce to topic
                    .AddOutbound<WebhookEvent>(endpoint => endpoint
                        .ProduceTo<WebhookEvent>(envelope =>
                        {
                            return $"{_producerOption.Value.Topic}-{envelope.Message.EventType}";
                        })));

        using (var scope = _provider.CreateScope())
        {
            var ser = scope.ServiceProvider;

            var pub = ser.GetRequiredService<Silverback.Messaging.Publishing.IPublisher>();
            var opt = ser.GetRequiredService<IOptions<WebhookOptions>>();
            foreach (var webhook in opt.Value.Methods)
            {
                pub.Publish(new WebhookEvent()
                {
                    EventType = webhook
                });
            }
        }
    }
}