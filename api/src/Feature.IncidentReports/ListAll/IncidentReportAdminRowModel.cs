using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.ListAll;

public record IncidentReportAdminRowModel
{
    public Guid IncidentReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = string.Empty;
    public TranslatedString FormName { get; init; } = null!;
    public string FormDefaultLanguage { get; init; } = string.Empty;

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; init; } = null!;

    public string? LocationDescription { get; init; }
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
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus FollowUpStatus { get; init; } = null!;

    public bool IsCompleted { get; init; }

    public IncidentReportAdminRowModel WithMaskedObserverData() => this with
    {
        ObserverName = PersonalDataMasker.MaskName(ObserverName),
        ObserverEmail = PersonalDataMasker.MaskEmail(ObserverEmail),
        ObserverPhoneNumber = PersonalDataMasker.MaskPhoneNumber(ObserverPhoneNumber)
    };
}
