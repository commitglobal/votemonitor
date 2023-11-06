#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Vote.Monitor.Api/Vote.Monitor.Api.csproj", "src/Vote.Monitor.Api/"]
COPY ["src/Vote.Monitor.Api.Authorization.Policies/Vote.Monitor.Api.Authorization.Policies.csproj", "src/Vote.Monitor.Api.Authorization.Policies/"]
COPY ["src/Vote.Monitor.Api.Feature.Auth/Vote.Monitor.Api.Feature.Auth.csproj", "src/Vote.Monitor.Api.Feature.Auth/"]
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Vote.Monitor.Api.Feature.Country/Vote.Monitor.Api.Feature.Country.csproj", "src/Vote.Monitor.Api.Feature.Country/"]
COPY ["src/Vote.Monitor.Api.Feature.CSOAdmin/Vote.Monitor.Api.Feature.CSOAdmin.csproj", "src/Vote.Monitor.Api.Feature.CSOAdmin/"]
COPY ["src/Vote.Monitor.Api.Feature.CSO/Vote.Monitor.Api.Feature.CSO.csproj", "src/Vote.Monitor.Api.Feature.CSO/"]
COPY ["src/Vote.Monitor.Api.Feature.ElectionRound.Domain/Vote.Monitor.Api.Feature.ElectionRound.Domain.csproj", "src/Vote.Monitor.Api.Feature.ElectionRound.Domain/"]
COPY ["src/Vote.Monitor.Api.Feature.ElectionRound/Vote.Monitor.Api.Feature.ElectionRound.csproj", "src/Vote.Monitor.Api.Feature.ElectionRound/"]
COPY ["src/Vote.Monitor.Api.Feature.Observer/Vote.Monitor.Api.Feature.Observer.csproj", "src/Vote.Monitor.Api.Feature.Observer/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation/Vote.Monitor.Api.Feature.PollingStation.csproj", "src/Vote.Monitor.Api.Feature.PollingStation/"]
RUN dotnet restore "src/Vote.Monitor.Api/Vote.Monitor.Api.csproj"
COPY . .
WORKDIR "/src/src/Vote.Monitor.Api"
RUN dotnet build "Vote.Monitor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vote.Monitor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vote.Monitor.Api.dll"]