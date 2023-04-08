Mluvii Webhook API Gateway
Overview
The Mluvii Webhook API Gateway is a service that receives webhook events from Mluvii and forwards them to a Kafka topic. It also supports health checks and automatic webhook registration.

Components
1. EndpointsConfigurator
The EndpointsConfigurator class configures Kafka endpoints for producers and consumers. It sets the bootstrap server address for connecting to Kafka and defines an outbound Kafka endpoint for producing WebhookEvent messages.

2. MluviiWebhook Controller
The MluviiWebhook controller handles incoming HTTP requests for webhook events. It supports two endpoints:

GET /MluviiWebhook: A simple health check endpoint that returns "Yes, I am alive!"
POST /MluviiWebhook: Handles incoming webhook events from Mluvii, deserializes the payload, and publishes it as a WebhookEvent message to the Kafka topic.
3. AutoRegisterWebhookJob
The AutoRegisterWebhookJob class is a Quartz job that automatically registers the webhook URL with Mluvii at a defined interval. It uses the WebhookRegistrator class to handle registration.

4. MluviiWebhookHealthCheck
The MluviiWebhookHealthCheck class implements a custom health check to verify the proper configuration of the service, including checking Kafka topic settings, Mluvii client communication, and webhook secret settings.

5. WebhookRegistrator
The WebhookRegistrator class handles the registration of webhook URLs with Mluvii. It can update an existing webhook or create a new one with the provided webhook URL and event types.

6. WebhookOptions
The WebhookOptions class represents the configuration options for the webhook service, including the webhook secret, automatic registration flag, event types to subscribe to, and the webhook URL.

Configuration
The service can be configured using the appsettings.json file. The main configuration sections include:

Logging: Logging settings for the application.
Kafka: Kafka connection settings, including the bootstrap server address.
KafkaProducer: Kafka producer settings, including the topic to produce messages to.
Webhook: Webhook configuration, including the secret, event types, automatic registration flag, and webhook URL.
FeatureManagement: Feature management settings.
Mluvii: Mluvii API settings, including the base API endpoint, token endpoint, client name, and secret.
Dockerfile
The provided Dockerfile builds a Docker image for the service. It uses the mcr.microsoft.com/dotnet/aspnet:6.0 and mcr.microsoft.com/dotnet/sdk:6.0 images as the base for runtime and build environments, respectively. The service is exposed on port 5025, and a health check is configured using curl to hit the /health endpoint.
