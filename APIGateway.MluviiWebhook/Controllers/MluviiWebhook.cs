using APIGateway.MluviiWebhook.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace APIGateway.MluviiWebhook.Controllers;

[ApiController]
[Route("MluviiWebhook")]
public class MluviiWebhook : ControllerBase
{
    private readonly ILogger<MluviiWebhook> _logger;
    private readonly IOptions<WebhookOptions> _webhookOptions;
    private readonly IPublisherFactory _publisherFactory;

    public MluviiWebhook(ILogger<MluviiWebhook> logger, IPublisherFactory publisherFactory,
        IOptions<WebhookOptions> webhookOptions)
    {
        _logger = logger;
        _publisherFactory = publisherFactory;
        _webhookOptions = webhookOptions;
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
            var publisher = await _publisherFactory.GetPublisher();
            await publisher.PublishAsync(jobj);
        }
        else
        {
            _logger.LogInformation("Bad request body:" + body);
            return BadRequest();
        }

        return Ok();
    }
}