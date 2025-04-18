FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
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
COPY ["src/Vote.Monitor.Domain/Vote.Monitor.Domain.csproj", "src/Vote.Monitor.Domain/"]
COPY ["src/Feature.Citizen.Guides/Feature.Citizen.Guides.csproj", "src/Feature.Citizen.Guides/"]
COPY ["src/Feature.Citizen.Notifications/Feature.Citizen.Notifications.csproj", "src/Feature.Citizen.Notifications/"]
COPY ["src/Feature.CitizenReports.Attachments/Feature.CitizenReports.Attachments.csproj", "src/Feature.CitizenReports.Attachments/"]
COPY ["src/Feature.CitizenReports.Notes/Feature.CitizenReports.Notes.csproj", "src/Feature.CitizenReports.Notes/"]
COPY ["src/Feature.CitizenReports/Feature.CitizenReports.csproj", "src/Feature.CitizenReports/"]
COPY ["src/Job.Contracts/Job.Contracts.csproj", "src/Job.Contracts/"]
COPY ["src/Module.Forms/Module.Forms.csproj", "src/Module.Forms/"]
COPY ["src/Module.Answers/Module.Answers.csproj", "src/Module.Answers/"]
COPY ["src/Feature.DataExport/Feature.DataExport.csproj", "src/Feature.DataExport/"]
COPY ["src/Feature.Feedback/Feature.Feedback.csproj", "src/Feature.Feedback/"]
COPY ["src/Feature.Form.Submissions/Feature.Form.Submissions.csproj", "src/Feature.Form.Submissions/"]
COPY ["src/Feature.Forms/Feature.Forms.csproj", "src/Feature.Forms/"]
COPY ["src/Feature.FormTemplates/Feature.FormTemplates.csproj", "src/Feature.FormTemplates/"]
COPY ["src/Feature.IncidentReports.Attachments/Feature.IncidentReports.Attachments.csproj", "src/Feature.IncidentReports.Attachments/"]
COPY ["src/Feature.IncidentReports.Notes/Feature.IncidentReports.Notes.csproj", "src/Feature.IncidentReports.Notes/"]
COPY ["src/Feature.IncidentReports/Feature.IncidentReports.csproj", "src/Feature.IncidentReports/"]
COPY ["src/Feature.Locations/Feature.Locations.csproj", "src/Feature.Locations/"]
COPY ["src/Feature.MonitoringObservers/Feature.MonitoringObservers.csproj", "src/Feature.MonitoringObservers/"]
COPY ["src/Feature.Coalitions/Feature.Coalitions.csproj", "src/Feature.Coalitions/"]
COPY ["src/Feature.ObserverGuide/Feature.ObserverGuide.csproj", "src/Feature.ObserverGuide/"]
COPY ["src/Feature.PollingStation.Information/Feature.PollingStation.Information.csproj", "src/Feature.PollingStation.Information/"]
COPY ["src/Feature.QuickReports/Feature.QuickReports.csproj", "src/Feature.QuickReports/"]
COPY ["src/Feature.Statistics/Feature.Statistics.csproj", "src/Feature.Statistics/"]
COPY ["src/Feature.UserPreferences/Feature.UserPreferences.csproj", "src/Feature.UserPreferences/"]
COPY ["src/Feature.PollingStations/Feature.PollingStations.csproj", "src/Feature.PollingStations/"]
COPY ["src/Feature.Observers/Feature.Observers.csproj", "src/Feature.Observers/"]
COPY ["src/Feature.Ngos/Feature.Ngos.csproj", "src/Feature.Ngos/"]
COPY ["src/Feature.Monitoring/Feature.Monitoring.csproj", "src/Feature.Monitoring/"]
COPY ["src/Feature.NgoAdmins/Feature.NgoAdmins.csproj", "src/Feature.NgoAdmins/"]
COPY ["src/Feature.Languages/Feature.Languages.csproj", "src/Feature.Languages/"]
COPY ["src/Feature.Notifications/Feature.Notifications.csproj", "src/Feature.Notifications/"]
COPY ["src/Feature.ElectionRounds/Feature.ElectionRounds.csproj", "src/Feature.ElectionRounds/"]
COPY ["src/Feature.Attachments/Feature.Attachments.csproj", "src/Feature.Attachments/"]
COPY ["src/Feature.Notes/Feature.Notes.csproj", "src/Feature.Notes/"]
COPY ["src/Feature.Countries/Feature.Countries.csproj", "src/Feature.Countries/"]
COPY ["src/Feature.Auth/Feature.Auth.csproj", "src/Feature.Auth/"]
COPY ["src/Feature.PollingStation.Visits/Feature.PollingStation.Visits.csproj", "src/Feature.PollingStation.Visits/"]
COPY ["src/Feature.PollingStation.Information.Forms/Feature.PollingStation.Information.Forms.csproj", "src/Feature.PollingStation.Information.Forms/"]
COPY ["src/Feature.DataCleanup/Feature.DataCleanup.csproj", "src/Feature.DataCleanup/"]
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