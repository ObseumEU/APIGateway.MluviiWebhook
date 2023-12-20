using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace APIGateway.MluviiWebhook.Tests;
public static class Handler
{
    public static WebApplicationFactory<Program> CreateTestServer(Action<IServiceCollection> services)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services);
            });
    }
}
