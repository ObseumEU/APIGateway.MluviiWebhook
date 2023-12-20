#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging. 
 

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base 
WORKDIR /app 
RUN apt-get update && apt-get --yes install curl
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR /src 
COPY . . 
RUN dotnet restore "APIGateway.MluviiWebhook/APIGateway.MluviiWebhook.csproj" 

WORKDIR "/src/" 
RUN dotnet build "APIGateway.MluviiWebhook/APIGateway.MluviiWebhook.csproj" -c Release -o /app/build 
 
FROM build AS publish 
RUN dotnet publish "APIGateway.MluviiWebhook/APIGateway.MluviiWebhook.csproj" -c Release -o /app/publish -f net6.0
 
ARG RUN_TESTS
RUN if [ "$RUN_TESTS" = "true" ]; then \
      dotnet test "APIGateway.MluviiWebhook.Tests/APIGateway.MluviiWebhook.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory:"/app/coverage/"; \
    fi

FROM base AS final 
WORKDIR /app 
COPY --from=publish /app/publish . 
ENV ASPNETCORE_ENVIRONMENT="Production" 
ENV ASPNETCORE_URLS="http://0.0.0.0:5025" 

EXPOSE 5025 

HEALTHCHECK --interval=30s --timeout=6s --retries=3 CMD curl --fail http://localhost:5025/health || exit 1

# Enable profiling
ENV CORECLR_ENABLE_PROFILING="1" \
    CORECLR_PROFILER="{E5A4ADC4-C749-400D-B066-6AC8C1D3790A}" \
    CORECLR_PROFILER_PATH_64="/YourKit-NetProfiler/bin/linux-x86-64/libynpagent.so" \
    YNP_STARTUP_OPTIONS="listen=all,port=10004"

ENTRYPOINT ["dotnet", "APIGateway.MluviiWebhook.dll"] 