#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Vote.Monitor.Api/Vote.Monitor.Api.csproj", "src/Vote.Monitor.Api/"]
COPY ["src/Vote.Monitor.Api.Authorization.Policies/Vote.Monitor.Api.Authorization.Policies.csproj", "src/Vote.Monitor.Api.Authorization.Policies/"]
COPY ["src/Vote.Monitor.Api.Feature.Auth/Vote.Monitor.Api.Feature.Auth.csproj", "src/Vote.Monitor.Api.Feature.Auth/"]
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Vote.Monitor.Api.Feature.Country/Vote.Monitor.Api.Feature.Country.csproj", "src/Vote.Monitor.Api.Feature.Country/"]
COPY ["src/Vote.Monitor.Api.Feature.Language/Vote.Monitor.Api.Feature.Language.csproj", "src/Vote.Monitor.Api.Feature.Language/"]
COPY ["src/Vote.Monitor.Api.Feature.CSOAdmin/Vote.Monitor.Api.Feature.CSOAdmin.csproj", "src/Vote.Monitor.Api.Feature.CSOAdmin/"]
COPY ["src/Vote.Monitor.Api.Feature.CSO/Vote.Monitor.Api.Feature.CSO.csproj", "src/Vote.Monitor.Api.Feature.CSO/"]
COPY ["src/Vote.Monitor.Api.Feature.ElectionRound/Vote.Monitor.Api.Feature.ElectionRound.csproj", "src/Vote.Monitor.Api.Feature.ElectionRound/"]
COPY ["src/Vote.Monitor.Api.Feature.Monitoring/Vote.Monitor.Api.Feature.Monitoring.csproj", "src/Vote.Monitor.Api.Feature.Monitoring/"]
COPY ["src/Vote.Monitor.Api.Feature.Observer/Vote.Monitor.Api.Feature.Observer.csproj", "src/Vote.Monitor.Api.Feature.Observer/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation/Vote.Monitor.Api.Feature.PollingStation.csproj", "src/Vote.Monitor.Api.Feature.PollingStation/"]
COPY ["src/Vote.Monitor.Api.Feature.UserPreferences/Vote.Monitor.Api.Feature.UserPreferences.csproj", "src/Vote.Monitor.Api.Feature.UserPreferences/"]

RUN dotnet restore "src/Vote.Monitor.Api/Vote.Monitor.Api.csproj"
COPY . .
WORKDIR "/src/src/Vote.Monitor.Api"
RUN dotnet build "Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vote.Monitor.Api.dll"]