using APIGateway.Core.Kafka.Messages;
using APIGateway.MluviiWebhook.Publisher;
using Newtonsoft.Json.Linq;
using Silverback.Messaging.Publishing;

namespace APIGateway.MluviiWebhook.Tests
{
    public class KafkaPublisherTests
    {
        private readonly Mock<IPublisher> _mockPublisher = new Mock<IPublisher>();
        private readonly KafkaPublisher _kafkaPublisher;

        public KafkaPublisherTests()
        {
            _kafkaPublisher = new KafkaPublisher(_mockPublisher.Object);
        }

        [Fact]
        public async Task PublishAsync_SendsCorrectMessage()
        {
            // Arrange
            var jobj = new JObject
            {
                ["eventType"] = "TestEvent",
                ["data"] = "TestData"
            };

            // Act
            await _kafkaPublisher.PublishAsync(jobj);

            // Assert
            _mockPublisher.Verify(
                x => x.PublishAsync(
                    It.Is<WebhookEvent>(we => we.EventType == "TestEvent" && we.JsonData == "TestData")
                ),
                Times.Once
            );
        }
    }
}
