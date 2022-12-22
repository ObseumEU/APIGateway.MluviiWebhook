#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APIGateway.MluviiWebhook.csproj", "."]
COPY ["APIGateway.Core/APIGateway.Core/APIGateway.Core/APIGateway.Core.csproj", "APIGateway.Core/APIGateway.Core/APIGateway.Core/"]
RUN dotnet restore "APIGateway.MluviiWebhook.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "APIGateway.MluviiWebhook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGateway.MluviiWebhook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.MluviiWebhook.dll"]
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_URLS="http://0.0.0.0:8080"
HEALTHCHECK --interval=30s --timeout=6s --retries=3 CMD curl --fail http://localhost:8080/health || exit 1