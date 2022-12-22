using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIGateway.MluviiWebhook.Tests;
public static class Handler
{
    public static WebApplicationFactory<Program> CreateTestServer(Action<IServiceCollection> services)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureServices(services);
            });
    }
}
