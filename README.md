# Mluvii Webhook API Gateway

Mluvii Webhook API Gateway is a .NET 6.0 application designed to act as a middleware for handling Mluvii Webhooks. It listens for incoming webhook events from Mluvii, validates them, and forwards them to a Kafka topic for further processing【9†source】.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Building and Running with Docker](#building-and-running-with-docker)
- [Health Check](#health-check)
- [Contributing](#contributing)
- [License](#license)

## Prerequisites
- .NET 6.0
- Kafka【9†source】

## Installation
1. Clone the repository
2. Build the solution
3. Configure the `appsettings.json` file (see [Configuration](#configuration) section for more details)
4. Run the project【9†source】

## Usage
After deploying the application, it will listen for incoming webhook events from Mluvii. Once a valid event is received, it will be forwarded to the configured Kafka topic【9†source】.

## Configuration
You can configure the application using the `appsettings.json` file. Here are the main settings you need to adjust:
- `Kafka`: Set the Kafka server host and port
- `KafkaProducer`: Set the Kafka topic where the webhook events will be sent
- `Webhook`: Set the webhook secret【9†source】

## Building and Running with Docker
The provided Dockerfile outlines the steps to build and run the application in a Docker container. The application runs on .NET 6.0 and listens on port 5025. It also includes a health check endpoint that can be accessed at `http://localhost:5025/health`【13†source】.

## Health Check
The application includes a health check endpoint at `http://localhost:5025/health`. This can be used to monitor the status of the application when it is running.

## Contributing
If you are interested in contributing to this project, please refer to the CONTRIBUTING.md file for guidelines.

## License
This project is licensed under the MIT License. Please see the LICENSE file for more details.
