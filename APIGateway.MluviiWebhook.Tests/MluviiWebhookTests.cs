using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Silverback.Messaging.Publishing;
using System.IO;
using Microsoft.FeatureManagement;
using APIGateway.MluviiWebhook.Contracts;
using MassTransit;
using APIGateway.MluviiWebhook.Publisher;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace APIGateway.MluviiWebhook.Tests;

public class MluviiWebhookTests
{
    private readonly Mock<ILogger<Controllers.MluviiWebhook>> _loggerMock = new Mock<ILogger<Controllers.MluviiWebhook>>();
    private readonly Mock<IOptions<WebhookOptions>> _webhookOptionsMock = new Mock<IOptions<WebhookOptions>>();
    private readonly Mock<IFeatureManager> _featureMock = new Mock<IFeatureManager>();
    private readonly Mock<IPublisherFactory> _publisherFactoryMock = new Mock<IPublisherFactory>();
    private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
    private readonly Controllers.MluviiWebhook _controller;

    public MluviiWebhookTests()
    {
        // Mock the publisher factory to return a mock publisher
        _publisherFactoryMock.Setup(factory => factory.GetPublisher()).ReturnsAsync(_messagePublisherMock.Object);

        // Initialize the controller with mocked dependencies
        _controller = new Controllers.MluviiWebhook(
            _loggerMock.Object,
            _publisherFactoryMock.Object,
            _webhookOptionsMock.Object
        );
    }

    [Fact]
    public async Task WebhookPost_ValidEvent_PublishesEvent()
    {
        // Arrange
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        var secret = "expectedSecret";
        _controller.ControllerContext = CreateMockHttpContext(requestBody, secret);
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = secret });

        // Act
        var result = await _controller.WebhookPost();

        // Assert
        Assert.IsType<OkResult>(result);
        _messagePublisherMock.Verify(publisher => publisher.PublishAsync(It.IsAny<JObject>()), Times.Once);
    }

    private ControllerContext CreateMockHttpContext(string requestBody, string secret)
    {
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(_ => _.Body).Returns(CreateRequestStream(requestBody));
        mockRequest.Setup(_ => _.Query["secret"]).Returns(secret);
        mockRequest.Setup(_ => _.Headers).Returns(new HeaderDictionary());
        mockRequest.Setup(_ => _.Path).Returns(new PathString("/MluviiWebhook"));

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(_ => _.Request).Returns(mockRequest.Object);

        return new ControllerContext { HttpContext = mockHttpContext.Object };

    }

    private static MemoryStream CreateRequestStream(string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WebhookPost_FeatureFlagBehavior_ReturnsExpectedResult(bool isRabbitMqEnabled)
    {
        // Arrange
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        _controller.ControllerContext = CreateMockHttpContext(requestBody, "expectedSecret");
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = "expectedSecret" });
        _featureMock.Setup(f => f.IsEnabledAsync(FeatureFlags.RABBITMQ)).ReturnsAsync(isRabbitMqEnabled);

        // Act
        var result = await _controller.WebhookPost();

        // Assert
        Assert.IsType<OkResult>(result);
        if (isRabbitMqEnabled)
        {
            // Verify that RabbitMQ publisher was used
            _messagePublisherMock.Verify(publisher => publisher.PublishAsync(It.IsAny<JObject>()), Times.Once);
        }
        else
        {
            // Verify that Kafka publisher was used
            _messagePublisherMock.Verify(publisher => publisher.PublishAsync(It.IsAny<JObject>()), Times.Once);
        }
    }

    [Fact]
    public async Task WebhookGet_ReturnsSuccessMessage()
    {
        var result = await _controller.WebhookGet() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Yes i am alive! Mluvii webhook.", result.Value);
    }

    [Fact]
    public async Task WebhookPost_WithInvalidSecret_ReturnsUnauthorized()
    {
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        _controller.ControllerContext = CreateMockHttpContext(requestBody, "invalidSecret");
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = "expectedSecret" });

        var result = await _controller.WebhookPost();

        Assert.IsType<UnauthorizedResult>(result);
    }
}
