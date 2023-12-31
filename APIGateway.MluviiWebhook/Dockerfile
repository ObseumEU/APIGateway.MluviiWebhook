# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base 
WORKDIR /app 
EXPOSE 5025 

# Install curl and clean up in one layer to reduce size
RUN apt-get update && apt-get --yes install curl && rm -rf /var/lib/apt/lists/*

# SDK Image for building source
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build 
WORKDIR /src 

# Copies the csproj and restores as distinct layers
COPY ["APIGateway.MluviiWebhook/APIGateway.MluviiWebhook.csproj", "APIGateway.MluviiWebhook/"]
RUN dotnet restore "APIGateway.MluviiWebhook/APIGateway.MluviiWebhook.csproj"

COPY . .
WORKDIR "/src/APIGateway.MluviiWebhook"
RUN dotnet build "APIGateway.MluviiWebhook.csproj" -c Release -o /app/build 

# Publish
FROM build AS publish 
RUN dotnet publish "APIGateway.MluviiWebhook.csproj" -c Release -o /app/publish -f net7.0

# Test
FROM build AS test 
WORKDIR "/src/"
RUN dotnet test "APIGateway.MluviiWebhook.Tests/APIGateway.MluviiWebhook.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory:"/app/coverage/";

# Final Image
FROM base AS final 
WORKDIR /app 
COPY --from=publish /app/publish . 
COPY --from=test /app/coverage ./coverage

ENV ASPNETCORE_ENVIRONMENT="Production" 
ENV ASPNETCORE_URLS="http://0.0.0.0:5025" 
HEALTHCHECK --interval=30s --timeout=6s --retries=3 CMD curl --fail http://localhost:5025/health || exit 1

ENTRYPOINT ["dotnet", "APIGateway.MluviiWebhook.dll"]
