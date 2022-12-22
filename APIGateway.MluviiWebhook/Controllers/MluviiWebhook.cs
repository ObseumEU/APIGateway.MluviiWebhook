using System.ComponentModel.DataAnnotations;
using System.Text;
using APIGateway.Core.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Logging;

namespace APIGateway.MluviiWebhook.Controllers
{
    [ApiController]
    [Route("MluviiWebhook")]
    public class MluviiWebhook : ControllerBase
    {
        private readonly ILogger<MluviiWebhook> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IOptions<KafkaProduceOption> _producer;
        private readonly IOptions<WebhookOptions> _webhookOptions;

        public MluviiWebhook(ILogger<MluviiWebhook> logger, IMessageBroker messageBroker , IOptions<KafkaProduceOption> producer, IOptions<WebhookOptions> webhookOptions)
        {
            _logger = logger;
            _messageBroker = messageBroker;
            _producer = producer;
            _webhookOptions = webhookOptions;
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

            var jobj = JsonConvert.DeserializeObject<JObject>(body);

            if (jobj != null && jobj["eventType"] != null)
            {
                var eventType = jobj["eventType"].ToString();
                //Reflex find object type
                var typeName = $"mluvii.ApiModels.Webhooks.Payloads.{ eventType }Payload";
                Type? bodyType = Type.GetType($"{typeName}, mluvii.ApiModels, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                if (bodyType == null)
                {
                    _logger.LogInformation("Bad request body:" + body);
                    return BadRequest();
                }

                var payload = jobj["data"].ToObject(bodyType); //Sanitize json, only for allowed payload

                if (string.IsNullOrEmpty(_producer.Value.Topic))
                    throw new Exception("Topic cannot be null");
                await _messageBroker.ProduceMessage($"{_producer.Value.Topic}{eventType}", payload);
            }
            else
            {
                _logger.LogInformation("Bad request body:" + body);
                return BadRequest();
            }
            
            return Ok();
        }
    }
}