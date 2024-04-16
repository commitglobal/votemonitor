#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Vote.Monitor.Hangfire/Vote.Monitor.Hangfire.csproj", "src/Vote.Monitor.Hangfire/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Job.Contracts/Job.Contracts.csproj", "src/Job.Contracts/"]
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
RUN dotnet restore "./src/Vote.Monitor.Hangfire/./Vote.Monitor.Hangfire.csproj"
COPY . .
WORKDIR "/src/src/Vote.Monitor.Hangfire"
RUN dotnet build "./Vote.Monitor.Hangfire.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vote.Monitor.Hangfire.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vote.Monitor.Hangfire.dll"]