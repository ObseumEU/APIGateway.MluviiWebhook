using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Silverback.Messaging.Publishing;
using System.IO;
using Microsoft.FeatureManagement;
using APIGateway.MluviiWebhook.Contracts;
using MassTransit;

namespace APIGateway.MluviiWebhook.Tests;

public class MluviiWebhookTests
{
    private readonly Mock<ILogger<Controllers.MluviiWebhook>> _loggerMock = new Mock<ILogger<Controllers.MluviiWebhook>>();
    private readonly Mock<IPublisher> _messageBrokerMock = new Mock<IPublisher>();
    private readonly Mock<IOptions<WebhookOptions>> _webhookOptionsMock = new Mock<IOptions<WebhookOptions>>();
    private readonly Mock<IFeatureManager> _featureMock = new Mock<IFeatureManager>();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new Mock<IPublishEndpoint>();
    private readonly Controllers.MluviiWebhook _controller;

    public MluviiWebhookTests()
    {
        _controller = new Controllers.MluviiWebhook(
            _loggerMock.Object,
            _messageBrokerMock.Object,
            _webhookOptionsMock.Object,
            _featureMock.Object,
            _publishEndpointMock.Object
        );
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

    [Fact]
    public async Task WebhookGet_ReturnsSuccessMessage()
    {
        var result = await _controller.WebhookGet() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("Yes i am alive! Mluvii webhook.", result.Value);
    }

    [Fact]
    public async Task WebhookPost_WithValidData_ReturnsOk()
    {
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        _controller.ControllerContext = CreateMockHttpContext(requestBody, "expectedSecret");
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = "expectedSecret" });

        var result = await _controller.WebhookPost() as OkResult;

        Assert.NotNull(result);
        _messageBrokerMock.Verify(broker => broker.PublishAsync(It.IsAny<WebhookEvent>()), Times.Once);
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

    [Fact]
    public async Task WebhookPost_WithRabbitMQEnabled_PublishesEventToRabbitMQ()
    {
        // Arrange
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        var secret = "expectedSecret";

        _controller.ControllerContext = CreateMockHttpContext(requestBody, secret);
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = secret });
        _featureMock.Setup(f => f.IsEnabledAsync(FeatureFlags.RABBITMQ)).ReturnsAsync(true);
        _publishEndpointMock.Setup(x => x.Publish(It.IsAny<WebhookEvent>(), default)).Verifiable();

        // Act
        var result = await _controller.WebhookPost();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<WebhookEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task WebhookPost_RabbitMQDisabled_DoesNotPublishEvent()
    {
        // Arrange
        var requestBody = "{\"eventType\": \"TestEvent\", \"data\": {}}";
        var secret = "expectedSecret";

        _controller.ControllerContext = CreateMockHttpContext(requestBody, secret);
        _webhookOptionsMock.Setup(_ => _.Value).Returns(new WebhookOptions { Secret = secret });
        _featureMock.Setup(f => f.IsEnabledAsync(FeatureFlags.RABBITMQ)).ReturnsAsync(false);

        // Act
        var result = await _controller.WebhookPost();

        // Assert
        Assert.IsType<OkResult>(result);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<WebhookEvent>(), default), Times.Never);
    }
}
