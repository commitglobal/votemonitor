#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Vote.Monitor.Api/Vote.Monitor.Api.csproj", "src/Vote.Monitor.Api/"]
COPY ["src/Vote.Monitor.Feature.PollingStation/Vote.Monitor.Feature.PollingStation.csproj", "src/Vote.Monitor.Feature.PollingStation/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Vote.Monitor.Feature.Example/Vote.Monitor.Feature.Example.csproj", "src/Vote.Monitor.Feature.Example/"]
COPY ["src/Vote.Monitor.Infra/Vote.Monitor.Infra.csproj", "src/Vote.Monitor.Infra/"]
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