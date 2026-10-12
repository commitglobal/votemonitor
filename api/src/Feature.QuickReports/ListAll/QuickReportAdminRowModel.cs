using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.ListAll;

public record QuickReportAdminRowModel
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public string Title { get; init; } = string.Empty;

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentCategory, string>))]
    public IncidentCategory IncidentCategory { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; init; } = null!;

    public string? PollingStationDetails { get; init; }
    public Guid? PollingStationId { get; init; }
    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string? Number { get; init; }
    public string? Address { get; init; }
    public Guid MonitoringNgoId { get; init; }
    public string NgoName { get; init; } = string.Empty;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = string.Empty;
    public string ObserverEmail { get; init; } = string.Empty;
    public string ObserverPhoneNumber { get; init; } = string.Empty;

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportFollowUpStatus, string>))]
    public QuickReportFollowUpStatus FollowUpStatus { get; init; } = null!;

    public QuickReportAdminRowModel WithMaskedObserverData() => this with
    {
        ObserverName = PersonalDataMasker.MaskName(ObserverName),
        ObserverEmail = PersonalDataMasker.MaskEmail(ObserverEmail),
        ObserverPhoneNumber = PersonalDataMasker.MaskPhoneNumber(ObserverPhoneNumber)
    };
}
