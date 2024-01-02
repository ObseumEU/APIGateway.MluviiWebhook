![FOMODOG](https://socialify.git.ci/BooAIPublic/APIGateway.MluviiWebhook/image?forks=1&issues=1&language=1&name=1&pulls=1&stargazers=1&theme=Light)
[![Continous Integration](https://github.com/BooAIPublic/APIGateway.MluviiWebhook/actions/workflows/main.yaml/badge.svg)](https://github.com/BooAIPublic/APIGateway.MluviiWebhook/actions/workflows/main.yaml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/68c366998df247e09411cf4abc6a5c4a)](https://app.codacy.com/gh/ObseumEU/APIGateway.MluviiWebhook/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Renovate enabled](https://img.shields.io/badge/renovate-enabled-brightgreen.svg)](https://renovatebot.com/)
[![codecov](https://codecov.io/github/ObseumEU/APIGateway.MluviiWebhook/graph/badge.svg?token=A55O60A046)](https://codecov.io/github/ObseumEU/APIGateway.MluviiWebhook)

# APIGateway.MluviiWebhook

## Overview

APIGateway.MluviiWebhook is a .NET 7 project designed to integrate with Mluvii's webhook system. It provides a flexible and robust way to handle events from Mluvii, a platform for customer service and communication. The project includes a Dockerfile for easy deployment and is designed with extensibility and maintainability in mind.

## Features

- **Webhook Integration**: Easily receive and process webhook events from Mluvii.
- **Logging**: Integrated logging with `ILogger`, providing insights into the application's operation.
- **Health Checks**: Includes a health check endpoint for monitoring the service's status.
- **OpenTelemetry Integration**: Optional telemetry for monitoring and observability.
- **Kafka Integration**: Outbound event publishing to Kafka topics.
- **RabbitMQ Integration**: Optional configuration for using RabbitMQ as a message broker.
- **Feature Flags**: Use of feature flags for enabling or disabling components like OpenTelemetry and RabbitMQ.

## Configuration

- `appsettings.json`: Configure Kafka, RabbitMQ, Webhook, and other settings.
- `Dockerfile`: For containerization and deployment.
- `.csproj` file: Define project dependencies and settings.

## Getting Started

1. **Prerequisites**:
   - .NET 7 SDK
   - Docker (for containerization)
   - Kafka or RabbitMQ servers (if using those features)

2. **Setup**:
   - Clone the repository.
   - Configure `appsettings.json` with your specific settings for Kafka, RabbitMQ, Mluvii, and other components.

3. **Running the Application**:
   - Run `dotnet build` to build the project.
   - Run `dotnet run` to start the application locally.
   - Alternatively, use Docker to build and run the containerized application.

4. **Health Checks**:
   - Access the `/health` endpoint for health status.

5. **Logging**:
   - Check the console or configured log destination for logs.

## Development

- Extend or modify the existing controllers and services as per your business requirements.
- Add new features and integrations as needed.
- Write and run tests for new and existing functionalities.

## Deployment

- Use the provided Dockerfile for containerization.
- Deploy to a container orchestration platform like Kubernetes, Docker Swarm, or a cloud provider's service.

## Contributing

- Contributions are welcome. Please fork the repository and submit pull requests with your enhancements.

## Support

- For support, please open an issue in the repository or contact the maintainers.
