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
COPY ["src/Authorization.Policies/Authorization.Policies.csproj", "src/Authorization.Policies/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
COPY ["src/Feature.ObserverGuide/Feature.ObserverGuide.csproj", "src/Feature.ObserverGuide/"]
COPY ["src/Feature.PollingStation.Information.Form/Feature.PollingStation.Information.Form.csproj", "src/Feature.PollingStation.Information.Form/"]
COPY ["src/Vote.Monitor.Form.Module/Vote.Monitor.Form.Module.csproj", "src/Vote.Monitor.Form.Module/"]
COPY ["src/Feature.PollingStation.Visit/Feature.PollingStation.Visit.csproj", "src/Feature.PollingStation.Visit/"]
COPY ["src/Vote.Monitor.Api.Feature.Answers.Attachments/Vote.Monitor.Api.Feature.Answers.Attachments.csproj", "src/Vote.Monitor.Api.Feature.Answers.Attachments/"]
COPY ["src/Vote.Monitor.Api.Feature.Answers.Notes/Vote.Monitor.Api.Feature.Answers.Notes.csproj", "src/Vote.Monitor.Api.Feature.Answers.Notes/"]
COPY ["src/Vote.Monitor.Api.Feature.Auth/Vote.Monitor.Api.Feature.Auth.csproj", "src/Vote.Monitor.Api.Feature.Auth/"]
COPY ["src/Vote.Monitor.Api.Feature.Country/Vote.Monitor.Api.Feature.Country.csproj", "src/Vote.Monitor.Api.Feature.Country/"]
COPY ["src/Vote.Monitor.Api.Feature.Emergencies.Attachments/Vote.Monitor.Api.Feature.Emergencies.Attachments.csproj", "src/Vote.Monitor.Api.Feature.Emergencies.Attachments/"]
COPY ["src/Vote.Monitor.Api.Feature.Emergencies/Vote.Monitor.Api.Feature.Emergencies.csproj", "src/Vote.Monitor.Api.Feature.Emergencies/"]
COPY ["src/Vote.Monitor.Api.Feature.FormTemplate/Vote.Monitor.Api.Feature.FormTemplate.csproj", "src/Vote.Monitor.Api.Feature.FormTemplate/"]
COPY ["src/Vote.Monitor.Api.Feature.Form/Vote.Monitor.Api.Feature.Form.csproj", "src/Vote.Monitor.Api.Feature.Form/"]
COPY ["src/Vote.Monitor.Api.Feature.Language/Vote.Monitor.Api.Feature.Language.csproj", "src/Vote.Monitor.Api.Feature.Language/"]
COPY ["src/Vote.Monitor.Api.Feature.NgoAdmin/Vote.Monitor.Api.Feature.NgoAdmin.csproj", "src/Vote.Monitor.Api.Feature.NgoAdmin/"]
COPY ["src/Vote.Monitor.Api.Feature.Ngo/Vote.Monitor.Api.Feature.Ngo.csproj", "src/Vote.Monitor.Api.Feature.Ngo/"]
COPY ["src/Vote.Monitor.Api.Feature.ElectionRound/Vote.Monitor.Api.Feature.ElectionRound.csproj", "src/Vote.Monitor.Api.Feature.ElectionRound/"]
COPY ["src/Vote.Monitor.Api.Feature.Monitoring/Vote.Monitor.Api.Feature.Monitoring.csproj", "src/Vote.Monitor.Api.Feature.Monitoring/"]
COPY ["src/Vote.Monitor.Api.Feature.Notifications/Vote.Monitor.Api.Feature.Notifications.csproj", "src/Vote.Monitor.Api.Feature.Notifications/"]
COPY ["src/Vote.Monitor.Api.Feature.Observer/Vote.Monitor.Api.Feature.Observer.csproj", "src/Vote.Monitor.Api.Feature.Observer/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation.Attachments/Vote.Monitor.Api.Feature.PollingStation.Attachments.csproj", "src/Vote.Monitor.Api.Feature.PollingStation.Attachments/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation/Vote.Monitor.Api.Feature.PollingStation.csproj", "src/Vote.Monitor.Api.Feature.PollingStation/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation.Information/Vote.Monitor.Api.Feature.PollingStation.Information.csproj", "src/Vote.Monitor.Api.Feature.PollingStation.Information/"]
COPY ["src/Vote.Monitor.Answer.Module/Vote.Monitor.Answer.Module.csproj", "src/Vote.Monitor.Answer.Module/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation.Notes/Vote.Monitor.Api.Feature.PollingStation.Notes.csproj", "src/Vote.Monitor.Api.Feature.PollingStation.Notes/"]
COPY ["src/Vote.Monitor.Api.Feature.UserPreferences/Vote.Monitor.Api.Feature.UserPreferences.csproj", "src/Vote.Monitor.Api.Feature.UserPreferences/"]
RUN dotnet restore "./src/Vote.Monitor.Api/./Vote.Monitor.Api.csproj"
COPY . .
WORKDIR "/src/src/Vote.Monitor.Api"
RUN dotnet build "./Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG REVISION=UNKNOWN
WORKDIR /app
COPY --from=publish /app/publish .

ENV COMMIT_HASH=$REVISION

ENTRYPOINT ["dotnet", "Vote.Monitor.Api.dll"]
