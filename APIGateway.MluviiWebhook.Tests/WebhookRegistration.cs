using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using APIGateway.Core.MluviiClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using mluvii.ApiModels.Webhooks;
using RestSharp;

namespace APIGateway.MluviiWebhook.Tests;

public class WebhookRegistration : IClassFixture<ApplicationFixture>
{
    [Fact]
    public async Task Can_autoregister_webhook_on_clean_mluvii_wit_secret()
    {
        var mluvii = new Mock<IMluviiClient>();
        var response = MoqIRestResponse();
        var webhooks = new List<WebhookModel>()
        {

        };
        mluvii.Setup(_ => _.AddWebhook(It.IsAny<string>(), It.IsAny<List<string>>()))
            .Callback<string, List<string>>((i, s) =>
            {
                i.Should().Be("https://foo.com&secret=TestSecret");
                s.Count.Should().Be(1);
                s.FirstOrDefault().Should().Be("sessionCreated");
            });
        mluvii.Setup(_ => _.GetWebhooks()).ReturnsAsync(() => (webhooks, response.Object));

        IOptions<WebhookOptions> option = Options.Create(new WebhookOptions()
        {
            AutoRegister = true,
            Methods = new string[] { "sessionCreated" },
            Secret = "TestSecret",
            WebhookUrl = "https://foo.com"
        });

        WebhookRegistrator registrator = new WebhookRegistrator(option, mluvii.Object);
        await registrator.RegisterWebhooks();
        await registrator.RegisterWebhooks(); //Try more times
        await registrator.RegisterWebhooks(); //Try more times
    }

    [Fact]
    public async Task Can_autoregister_webhook_on_clean_mluvii_without_secret()
    {
        var mluvii = new Mock<IMluviiClient>();
        var response = MoqIRestResponse();
        var webhooks = new List<WebhookModel>()
        {

        };
        mluvii.Setup(_ => _.AddWebhook(It.IsAny<string>(), It.IsAny<List<string>>()))
            .Callback<string, List<string>>((i, s) =>
            {
                i.Should().Be("https://foo.com");
                s.Count.Should().Be(1);
                s.FirstOrDefault().Should().Be("sessionCreated");
            });
        mluvii.Setup(_ => _.GetWebhooks()).ReturnsAsync(() => (webhooks, response.Object));

        IOptions<WebhookOptions> option = Options.Create(new WebhookOptions()
        {
            AutoRegister = true,
            Methods = new string[] { "sessionCreated" },
            WebhookUrl = "https://foo.com"
        });

        WebhookRegistrator registrator = new WebhookRegistrator(option, mluvii.Object);
        await registrator.RegisterWebhooks();
        await registrator.RegisterWebhooks(); //Try more times
        await registrator.RegisterWebhooks(); //Try more times
    }

    [Fact]
    public async Task Can_update_in_existing_webhooks()
    {
        var mluvii = new Mock<IMluviiClient>();
        var response = MoqIRestResponse();
        var webhooks = new List<WebhookModel>()
        {
            new WebhookModel()
            {
                CallbackUrl = "https://foo.com",
                EventTypes = new List<WebhookEventType>()
                {
                    WebhookEventType.ApplicationSettingChanged,
                    WebhookEventType.EmailThreadCreated,
                    WebhookEventType.EmailThreadOperatorJoined,
                    WebhookEventType.SessionActivityAutoActivity
                },
                ID = 1
            },
            new WebhookModel()
            {
                CallbackUrl = "https://foo1.com",
                EventTypes = new List<WebhookEventType>()
                {
                    WebhookEventType.ApplicationSettingChanged,
                    WebhookEventType.EmailThreadCreated,
                    WebhookEventType.EmailThreadOperatorJoined,
                    WebhookEventType.SessionActivityAutoActivity
                },
                ID = 2
            }
            ,            new WebhookModel()
            {
                CallbackUrl = "https://foo2.com",
                EventTypes = new List<WebhookEventType>()
                {
                    WebhookEventType.ApplicationSettingChanged,
                    WebhookEventType.EmailThreadCreated,
                    WebhookEventType.EmailThreadOperatorJoined,
                    WebhookEventType.SessionActivityAutoActivity
                },
                ID = 3
            }
        };
        mluvii.Setup(_ => _.UpdateWebhook(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<List<string>>()))
            .Callback<int, string, List<string>>((id, callback, events) =>
            {
                id.Should().Be(1);
                callback.Should().Be("https://foo.com");
                events.Count.Should().Be(1);
                events.FirstOrDefault().Should().Be("sessionCreated");
            });
        mluvii.Setup(_ => _.GetWebhooks()).ReturnsAsync(() => (webhooks, response.Object));

        IOptions<WebhookOptions> option = Options.Create(new WebhookOptions()
        {
            AutoRegister = true,
            Methods = new string[] { "sessionCreated" },
            WebhookUrl = "https://foo.com"
        });

        WebhookRegistrator registrator = new WebhookRegistrator(option, mluvii.Object);
        await registrator.RegisterWebhooks();
        await registrator.RegisterWebhooks(); //Try more times
        await registrator.RegisterWebhooks(); //Try more times
    }


    [Fact]
    public async Task Can_add_in_existing_webhooks()
    {
        var mluvii = new Mock<IMluviiClient>();
        var response = MoqIRestResponse();
        var webhooks = new List<WebhookModel>()
        {
            new WebhookModel()
            {
                CallbackUrl = "https://foo.com",
                EventTypes = new List<WebhookEventType>()
                {
                    WebhookEventType.ApplicationSettingChanged,
                    WebhookEventType.EmailThreadCreated,
                    WebhookEventType.EmailThreadOperatorJoined,
                    WebhookEventType.SessionActivityAutoActivity
                },
                ID = 1
            }
        };


        mluvii.Setup(_ => _.AddWebhook(It.IsAny<string>(), It.IsAny<List<string>>()))
            .Callback<string, List<string>>((id, events) =>
            {
                id.Should().Be("https://foo2.com");
                events.Count.Should().Be(3);
                events.FirstOrDefault().Should().Be("sessionCreated");
            });
        mluvii.Setup(_ => _.GetWebhooks()).ReturnsAsync(() => (webhooks, response.Object));

        IOptions<WebhookOptions> option = Options.Create(new WebhookOptions()
        {
            AutoRegister = true,
            Methods = new string[] { "sessionCreated" , "test1" , "test2"},
            WebhookUrl = "https://foo2.com"
        });

        WebhookRegistrator registrator = new WebhookRegistrator(option, mluvii.Object);
        await registrator.RegisterWebhooks();
        await registrator.RegisterWebhooks(); //Try more times
        await registrator.RegisterWebhooks(); //Try more times
    }

    private static Mock<IRestResponse> MoqIRestResponse()
    {
        var response = new Mock<IRestResponse>();
        response.SetupGet(_ => _.IsSuccessful).Returns(true);
        return response;
    }
}