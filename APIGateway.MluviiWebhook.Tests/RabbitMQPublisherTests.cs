using APIGateway.MluviiWebhook.Contracts;
using APIGateway.MluviiWebhook.Publisher;
using MassTransit;
using Newtonsoft.Json.Linq;

namespace APIGateway.MluviiWebhook.Tests
{
    public class RabbitMQPublisherTests
    {
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public RabbitMQPublisherTests()
        {
            // Setup the RabbitMQPublisher with a mock IPublishEndpoint
            _rabbitMQPublisher = new RabbitMQPublisher(_mockPublishEndpoint.Object);
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
            await _rabbitMQPublisher.PublishAsync(jobj);

            // Assert
            _mockPublishEndpoint.Verify(
                x => x.Publish<WebhookEvent>(
                    It.Is<WebhookEvent>(we => we.EventType == "TestEvent" && we.JsonData == "TestData"),
                    default // or a specific CancellationToken if your method supports it
                ),
                Times.Once
            );
        }
    }
}
