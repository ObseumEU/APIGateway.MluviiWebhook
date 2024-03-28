using MassTransit;
using Newtonsoft.Json.Linq;
using APIGateway.MluviiWebhook.Publisher;
using APIGateway.MluviiWebhook.Contracts;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace APIGateway.MluviiWebhook.Tests;

public class RabbitMQPublisherTests
{
    [Fact]
    public async Task PublishAsync_PublishesCorrectMessageToRabbitMQ()
    {
        // Arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var logMock = new Mock<ILogger<RabbitMQPublisher>>();
        var rabbitMQPublisher = new RabbitMQPublisher(publishEndpointMock.Object, logMock.Object);

        var eventType = "TestEvent";
        var jsonData = "TestData";
        var jobj = new JObject
        {
            ["eventType"] = eventType,
            ["data"] = jsonData
        };

        // Act
        await rabbitMQPublisher.PublishAsync(jobj);

        // Assert
        publishEndpointMock.Verify(pe => pe.Publish<WebhookEventContract>(
            It.Is<WebhookEventContract>(we =>
                we.EventType == eventType &&
                we.JsonData == jsonData),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}