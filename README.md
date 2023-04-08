# APIGateway.MluviiWebhook Documentation

## Introduction

APIGateway.MluviiWebhook is a C# project that allows you to receive and handle webhook notifications from the Mluvii live chat software. This project is designed to run in a Docker container, making it easy to deploy and scale.

## Getting Started

### Prerequisites

- Docker

### Installation

1. Clone the repository to your local machine:

git clone https://github.com/BooAIPublic/APIGateway.MluviiWebhook.git

arduino
Copy code

2. Build the Docker image:

docker build -t apigateway-mluvii-webhook .

markdown
Copy code

3. Run the Docker container:

docker run -p 8080:80 -d apigateway-mluvii-webhook

javascript
Copy code

### Configuration

The APIGateway.MluviiWebhook project uses environment variables for configuration. The following environment variables are required:

- `MluviiApiUrl`: The URL of the Mluvii API endpoint.
- `MluviiApiToken`: The API token used for authentication with the Mluvii API.

You can set these environment variables in your Docker container by passing them in as arguments to the `docker run` command:

docker run -p 8080:80 -d -e MluviiApiUrl=<url> -e MluviiApiToken=<token> apigateway-mluvii-webhook

csharp
Copy code

## API Documentation

The APIGateway.MluviiWebhook project provides the following endpoints:

### `POST /webhook`

This endpoint is used to receive webhook notifications from the Mluvii live chat software. The payload of the webhook notification is sent in the body of the request.

#### Request Format

POST /webhook HTTP/1.1
Content-Type: application/json

{
"eventType": "string",
"chatId": "string",
"data": { ... }
}

r
Copy code

#### Response Format

The response to the webhook notification should be an HTTP 200 OK status code.

## Contributing

Contributions to APIGateway.MluviiWebhook are welcome! Please submit any pull requests to the `main` branch.

## Troubleshooting

### "Unable to connect to the Mluvii API"

This error can occur if the `MluviiApiUrl` or `MluviiApiToken` environment variables are not set correctly. Please check your configuration and try again.

## Limitations and Known Issues

- The APIGateway.MluviiWebhook project currently only supports a subset of the Mluvii webhook notification types. Future updates may add support for additional notification types.
- The APIGateway.MluviiWebhook project does not currently include any authentication or authoriz
