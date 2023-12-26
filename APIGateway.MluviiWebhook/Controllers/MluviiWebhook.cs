using APIGateway.MluviiWebhook.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silverback.Messaging.Publishing;
using System.Text;

namespace APIGateway.MluviiWebhook.Controllers;

[ApiController]
[Route("MluviiWebhook")]
public class MluviiWebhook : ControllerBase
{
    private readonly ILogger<MluviiWebhook> _logger;
    private readonly IPublisher _messageBroker;
    private readonly IOptions<WebhookOptions> _webhookOptions;
    private readonly IFeatureManager _feature;

    public MluviiWebhook(ILogger<MluviiWebhook> logger, IPublisher messageBroker,
        IOptions<WebhookOptions> webhookOptions, IFeatureManager feature)
    {
        _logger = logger;
        _messageBroker = messageBroker;
        _webhookOptions = webhookOptions;
        _feature = feature;
    }

    [HttpGet]
    public async Task<ActionResult> WebhookGet()
    {
        return Ok("Yes i am alive! Mluvii webhook.");
    }

    [HttpPost]
    public async Task<ActionResult> WebhookPost()
    {
        if (!Request.Body.CanSeek)
        {
            Request.EnableBuffering();
        }

        //Check secret
        if (!string.IsNullOrEmpty(_webhookOptions.Value.Secret))
        {
            if (!Request.Query["secret"].Equals(_webhookOptions.Value.Secret))
            {
                _logger.LogWarning("Unauthorized access to webhook.");
                return Unauthorized();
            }
        }

        Request.Body.Position = 0;
        var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        Request.Body.Position = 0;
        _logger.LogInformation("Request body:" + body);

        var jobj = JsonConvert.DeserializeObject<JObject>(body);
        if (jobj != null && jobj["eventType"] != null)
        {
            await PublishKafkaEvent(jobj);

            if (await _feature.IsEnabledAsync(FeatureFlags.RABBITMQ))
            {
                await PublishRabbitMQEvent(jobj);
            }
        }
        else
        {
            _logger.LogInformation("Bad request body:" + body);
            return BadRequest();
        }

        return Ok();
    }

    private Task PublishRabbitMQEvent(JObject jobj)
    {
        throw new NotImplementedException();
    }

    private async Task PublishKafkaEvent(JObject jobj)
    {
        var payload = new WebhookEvent();
        payload.EventType = jobj["eventType"].ToString();
        payload.JsonData = jobj["data"].ToString();

        await _messageBroker.PublishAsync(payload);
    }
}