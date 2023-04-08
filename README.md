# Mluvii Webhook API Gateway

This project is an API Gateway for handling Mluvii Webhooks. It is designed to receive webhook events, validate them, and forward them to a Kafka topic for further processing.

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Health Check](#health-check)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

These instructions will help you set up and configure the API Gateway for Mluvii Webhooks.

### Prerequisites

- .NET 6.0
- Kafka

### Installation

1. Clone the repository
2. Build the solution
3. Configure the `appsettings.json` file (see [Configuration](#configuration) section for more details)
4. Run the project

## Usage

After deploying the application, it will listen for incoming webhook events from Mluvii. Once a valid event is received, it will be forwarded to the configured Kafka topic.

## Configuration

You can configure the application using the `appsettings.json` file. Here are the main settings you need to adjust:

- `Kafka`: Set the Kafka server host and port
- `KafkaProducer`: Set the Kafka topic where the webhook events will be sent
- `Webhook`: Set the webhook secret, methods, auto-register setting, and webhook URL
- `Mluvii`: Set the Mluvii API base URL, token endpoint, name, and secret

## Health Check

The application has a built-in health check endpoint at `/health`. This can be used to monitor the health of the API Gateway.

## Contributing

If you'd like to contribute to this project, feel free to submit a pull request or open an issue.

## Code Overview

### AutoRegisterWebhookJob

The `AutoRegisterWebhookJob` class is responsible for periodically registering the webhook with Mluvii. It uses a Quartz scheduler to execute the job on a specified interval.

### MluviiWebhookHealthCheck

The `MluviiWebhookHealthCheck` class is a health check implementation for the API Gateway. It checks the availability of the Mluvii client and Kafka settings to determine the health of the application.

### WebhookRegistrator

The `WebhookRegistrator` class is responsible for registering, updating, and managing the Mluvii webhook. It communicates with the Mluvii API to ensure the webhook is properly configured.

### WebhookOptions

The `WebhookOptions` class is a configuration object that holds various settings related to the webhook, such as the secret, auto-register setting, methods, and webhook URL.

### Startup Configuration

The startup configuration code sets up various services, such as feature management, logging, Kafka, and Mluvii client. It also configures health checks and the main application pipeline.

## Dockerfile

The provided Dockerfile is used to build and package the application in a Docker container. It uses the .NET 6.0 runtime and SDK images, and includes steps to build, test, and publish the application. The Dockerfile also defines a health check using the `/health` endpoint.
