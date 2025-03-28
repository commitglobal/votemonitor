﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/Vote.Monitor.Api/Vote.Monitor.Api.csproj", "src/Vote.Monitor.Api/"]
COPY ["src/Authorization.Policies/Authorization.Policies.csproj", "src/Authorization.Policies/"]
COPY ["src/Vote.Monitor.Core/Vote.Monitor.Core.csproj", "src/Vote.Monitor.Core/"]
COPY ["src/Job.Contracts/Job.Contracts.csproj", "src/Job.Contracts/"]
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
COPY ["src/Feature.Citizen.Guides/Feature.Citizen.Guides.csproj", "src/Feature.Citizen.Guides/"]
COPY ["src/Feature.CitizenReports.Attachments/Feature.CitizenReports.Attachments.csproj", "src/Feature.CitizenReports.Attachments/"]
COPY ["src/Feature.CitizenReports.Notes/Feature.CitizenReports.Notes.csproj", "src/Feature.CitizenReports.Notes/"]
COPY ["src/Feature.CitizenReports/Feature.CitizenReports.csproj", "src/Feature.CitizenReports/"]
COPY ["src/Vote.Monitor.Answer.Module/Vote.Monitor.Answer.Module.csproj", "src/Vote.Monitor.Answer.Module/"]
COPY ["src/Vote.Monitor.Form.Module/Vote.Monitor.Form.Module.csproj", "src/Vote.Monitor.Form.Module/"]
COPY ["src/Feature.DataExport/Feature.DataExport.csproj", "src/Feature.DataExport/"]
COPY ["src/Feature.Feedback/Feature.Feedback.csproj", "src/Feature.Feedback/"]
COPY ["src/Feature.Form.Submissions/Feature.Form.Submissions.csproj", "src/Feature.Form.Submissions/"]
COPY ["src/Feature.Forms/Feature.Forms.csproj", "src/Feature.Forms/"]
COPY ["src/Feature.FormTemplates/Feature.FormTemplates.csproj", "src/Feature.FormTemplates/"]
COPY ["src/Feature.ImportErrors/Feature.ImportErrors.csproj", "src/Feature.ImportErrors/"]
COPY ["src/Feature.IncidentReports.Attachments/Feature.IncidentReports.Attachments.csproj", "src/Feature.IncidentReports.Attachments/"]
COPY ["src/Feature.IncidentReports.Notes/Feature.IncidentReports.Notes.csproj", "src/Feature.IncidentReports.Notes/"]
COPY ["src/Feature.IncidentReports/Feature.IncidentReports.csproj", "src/Feature.IncidentReports/"]
COPY ["src/Feature.Locations/Feature.Locations.csproj", "src/Feature.Locations/"]
COPY ["src/Feature.MonitoringObservers/Feature.MonitoringObservers.csproj", "src/Feature.MonitoringObservers/"]
COPY ["src/Feature.ObserverGuide/Feature.ObserverGuide.csproj", "src/Feature.ObserverGuide/"]
COPY ["src/Feature.PollingStation.Information.Form/Feature.PollingStation.Information.Form.csproj", "src/Feature.PollingStation.Information.Form/"]
COPY ["src/Feature.PollingStation.Information/Feature.PollingStation.Information.csproj", "src/Feature.PollingStation.Information/"]
COPY ["src/Feature.PollingStation.Visit/Feature.PollingStation.Visit.csproj", "src/Feature.PollingStation.Visit/"]
COPY ["src/Feature.QuickReports/Feature.QuickReports.csproj", "src/Feature.QuickReports/"]
COPY ["src/Feature.Statistics/Feature.Statistics.csproj", "src/Feature.Statistics/"]
COPY ["src/Vote.Monitor.Api.Feature.Auth/Vote.Monitor.Api.Feature.Auth.csproj", "src/Vote.Monitor.Api.Feature.Auth/"]
COPY ["src/Vote.Monitor.Api.Feature.Country/Vote.Monitor.Api.Feature.Country.csproj", "src/Vote.Monitor.Api.Feature.Country/"]
COPY ["src/Vote.Monitor.Api.Feature.ElectionRound/Vote.Monitor.Api.Feature.ElectionRound.csproj", "src/Vote.Monitor.Api.Feature.ElectionRound/"]
COPY ["src/Vote.Monitor.Api.Feature.Language/Vote.Monitor.Api.Feature.Language.csproj", "src/Vote.Monitor.Api.Feature.Language/"]
COPY ["src/Feature.Monitoring/Feature.Monitoring.csproj", "src/Feature.Monitoring/"]
COPY ["src/Vote.Monitor.Api.Feature.NgoAdmin/Vote.Monitor.Api.Feature.NgoAdmin.csproj", "src/Vote.Monitor.Api.Feature.NgoAdmin/"]
COPY ["src/Vote.Monitor.Api.Feature.Ngo/Vote.Monitor.Api.Feature.Ngo.csproj", "src/Vote.Monitor.Api.Feature.Ngo/"]
COPY ["src/Feature.Notifications/Feature.Notifications.csproj", "src/Feature.Notifications/"]
COPY ["src/Vote.Monitor.Api.Feature.Observer/Vote.Monitor.Api.Feature.Observer.csproj", "src/Vote.Monitor.Api.Feature.Observer/"]
COPY ["src/Feature.Attachments/Feature.Attachments.csproj", "src/Feature.Attachments/"]
COPY ["src/Feature.Notes/Feature.Notes.csproj", "src/Feature.Notes/"]
COPY ["src/Vote.Monitor.Api.Feature.PollingStation/Vote.Monitor.Api.Feature.PollingStation.csproj", "src/Vote.Monitor.Api.Feature.PollingStation/"]
COPY ["src/Vote.Monitor.Api.Feature.UserPreferences/Vote.Monitor.Api.Feature.UserPreferences.csproj", "src/Vote.Monitor.Api.Feature.UserPreferences/"]
COPY ["src/Feature.Citizen.Notifications/Feature.Citizen.Notifications.csproj", "src/Feature.Citizen.Notifications/"]
COPY ["src/Feature.Coalitions/Feature.Coalitions.csproj", "src/Feature.Coalitions/"]

RUN dotnet restore "src/Vote.Monitor.Api/Vote.Monitor.Api.csproj"
COPY . .
WORKDIR "/src/src/Vote.Monitor.Api"
RUN dotnet build "Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Vote.Monitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG REVISION=UNKNOWN
WORKDIR /app
COPY --from=publish /app/publish .

ENV COMMIT_HASH=$REVISION

ENTRYPOINT ["dotnet", "Vote.Monitor.Api.dll"]
