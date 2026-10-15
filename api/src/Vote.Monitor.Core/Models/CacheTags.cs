namespace Vote.Monitor.Core.Models;

/// <summary>
/// Tags attached to cache entries, so that related entries can be invalidated together (RemoveByTagAsync).
/// </summary>
public static class CacheTags
{
    public const string Statistics = "statistics";
    public const string Forms = "forms";
    public const string Locations = "locations";
    public const string PollingStations = "polling-stations";

    public static string ElectionRound(Guid electionRoundId) => $"election-round:{electionRoundId}";
    public static string Ngo(Guid ngoId) => $"ngo:{ngoId}";
    public static string MonitoringNgo(Guid monitoringNgoId) => $"monitoring-ngo:{monitoringNgoId}";
    public static string MonitoringObserver(Guid monitoringObserverId) => $"monitoring-observer:{monitoringObserverId}";
    public static string Observer(Guid observerId) => $"observer:{observerId}";
    public static string Coalition(Guid coalitionId) => $"coalition:{coalitionId}";
    public static string Form(Guid formId) => $"form:{formId}";
    public static string PollingStation(Guid pollingStationId) => $"polling-station:{pollingStationId}";

    // Versioned data: when a version changes, the entries tagged with the previous version are removed.
    public static string LocationsVersion(Guid electionRoundId, Guid version) =>
        $"election-round:{electionRoundId}:locations-version:{version}";

    public static string PollingStationsVersion(Guid electionRoundId, Guid version) =>
        $"election-round:{electionRoundId}:polling-stations-version:{version}";

    public static string FormsVersion(Guid monitoringNgoId, Guid version) =>
        $"monitoring-ngo:{monitoringNgoId}:forms-version:{version}";
}
