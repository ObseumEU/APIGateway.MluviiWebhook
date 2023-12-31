#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["APIGateway.MluviiWebhook.Example.Consumer/APIGateway.MluviiWebhook.Example.Consumer.csproj", "APIGateway.MluviiWebhook.Example.Consumer/"]
COPY ["APIGateway.MluviiWebhook.Contracts/APIGateway.MluviiWebhook.Contracts.csproj", "APIGateway.MluviiWebhook.Contracts/"]
RUN dotnet restore "./APIGateway.MluviiWebhook.Example.Consumer/APIGateway.MluviiWebhook.Example.Consumer.csproj"
COPY . .
WORKDIR "/src/APIGateway.MluviiWebhook.Example.Consumer"
RUN dotnet build "./APIGateway.MluviiWebhook.Example.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./APIGateway.MluviiWebhook.Example.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .\

ENV ASPNETCORE_ENVIRONMENT="Production" 
ENV ASPNETCORE_URLS="http://0.0.0.0:80" 
ENTRYPOINT ["dotnet", "APIGateway.MluviiWebhook.Example.Consumer.dll"]