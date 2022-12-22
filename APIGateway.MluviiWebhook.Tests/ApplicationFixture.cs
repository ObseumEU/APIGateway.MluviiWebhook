using System;

namespace APIGateway.MluviiWebhook.Tests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

public class ApplicationFixture : IDisposable {
    public ApplicationFixture() {
        Application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureServices(services => {
                    services.AddMvc().AddApplicationPart(typeof(TestController).Assembly);
                });
            });
    }

    public readonly WebApplicationFactory<Program> Application;

    public TestServer Server => Application.Server;

    public void Dispose() {
        Application.Dispose();
    }
}

public class TestController : ControllerBase {
    [HttpGet("test")]
    public IActionResult Test() {
        return Content("Ok");
    }
}
