using APIGateway.MluviiWebhook.Example.Consumer;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostEnvironment env = Host.CreateDefaultBuilder(args).Build().Services.GetRequiredService<IHostEnvironment>();
new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, true)
    .AddEnvironmentVariables()
    .Build();


Host.CreateDefaultBuilder(args)
     .ConfigureServices((hostContext, services) =>
     {
         services.AddMassTransit(x =>
         {
             x.AddConsumer<MessageConsumer>();
             x.UsingRabbitMq((context, cfg) =>
             {
                 var rabbitMQConfig = context.GetRequiredService<IConfiguration>().GetSection("RabbitMQ");
                 cfg.Host(rabbitMQConfig["Host"], rabbitMQConfig["VirtualHost"], h =>
                 {
                     h.Username(rabbitMQConfig["Username"]);
                     h.Password(rabbitMQConfig["Password"]);
                 });

                 cfg.ConfigureEndpoints(context);
             });
         });
     })
     .Build()
     .Run();