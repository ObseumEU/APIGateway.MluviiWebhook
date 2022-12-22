using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using APIGateway.Core.Kafka;
using APIGateway.Core.Kafka.Messages;
using APIGateway.Core.MluviiClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using mluvii.ApiModels.Sessions;
using mluvii.ApiModels.Webhooks;
using mluvii.ApiModels.Webhooks.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace APIGateway.MluviiWebhook.Tests;

public class WebhookController : IClassFixture<ApplicationFixture>
{
    public WebhookController(ApplicationFixture fixture)
    {
        this.fixture = fixture;
    }

    readonly ApplicationFixture fixture;
    private TestServer Server => fixture.Server;

    [Fact]
    public async Task Test_missing_properties_in_webhook()
    {
        var req = Server.CreateRequest("/MluviiWebhook");
        var res = await req.PostAsync();
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Ensure_that_application_can_process_unknown_event_type()
    {
        WebhookEvent evnt = new WebhookEvent()
        {
            data = JObject.FromObject(new SessionCreatedPayload()
            {
                Id = 1,
                Channel = SessionChannel.Chat,
                Source = SessionSource.Default,
                TenantId = 1
            }),
            eventType = "UnknownEventTypeFoo"

        };
        var req = Server.CreateClient();

        var content = new StringContent(
            JsonConvert.SerializeObject(evnt),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var res = await req.PostAsync("/MluviiWebhook", content);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Test_simple_webhook_sessionCreated()
    {
        WebhookEvent evnt = new WebhookEvent()
        {
            data = JObject.FromObject(new SessionCreatedPayload()
            {
                Id = 1,
                Channel = SessionChannel.Chat,
                Source = SessionSource.Default,
                TenantId = 1
            }),
            eventType = "SessionCreated"

        };

        var broker = new Mock<IMessageBroker>();
        IOptions<KafkaProduceOption> options = Options.Create(new KafkaProduceOption()
        {
            Topic = "MluviiWebhook"
        });
        broker.Setup(_ => _.ProduceMessage(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((topic, message) =>
            {
                topic.Should().Be("MluviiWebhookSessionCreated");
                var res = JsonConvert.DeserializeObject<SessionCreatedPayload>(message.ToString()); //Test if its deserializable back to payload
            });
        var testClient = Handler.CreateTestServer(services =>
        {
            services.AddScoped<IMessageBroker>(provider => broker.Object);
            services.AddSingleton(options);
        }).CreateClient();
        
        var content = new StringContent(
            JsonConvert.SerializeObject(evnt),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var res = await testClient.PostAsync("/MluviiWebhook", content);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}