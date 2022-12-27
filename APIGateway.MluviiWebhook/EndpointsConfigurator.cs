using APIGateway.Core.Kafka;
using Microsoft.Extensions.Options;
using Silverback.Messaging.Configuration;

namespace Silverback.Samples.Kafka.Batch.Producer
{
    public class EndpointsConfigurator : IEndpointsConfigurator
    {
        private readonly IOptions<KafkaOption> _kafkaOption;
        private readonly IOptions<KafkaProduceOption> _producerOption;

        public EndpointsConfigurator(IOptions<KafkaOption> kafkaOption, IOptions<KafkaProduceOption> producerOption)
        {
            _kafkaOption = kafkaOption;
            _producerOption = producerOption;
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
                        .AddOutbound<APIGateway.Core.Kafka.Messages.WebhookEvent>(endpoint => endpoint
                            .ProduceTo<APIGateway.Core.Kafka.Messages.WebhookEvent>(envelope =>
                            {
                                return $"{_producerOption.Value.Topic}-{envelope.Message.EventType}";
                            })));
        }
    }
}
