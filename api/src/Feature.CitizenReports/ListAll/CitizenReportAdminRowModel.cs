using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.ListAll;

public record CitizenReportAdminRowModel
{
    public Guid CitizenReportId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = string.Empty;
    public TranslatedString FormName { get; init; } = null!;
    public string FormDefaultLanguage { get; init; } = string.Empty;
    public Guid LocationId { get; init; }
    public string Level1 { get; init; } = string.Empty;
    public string Level2 { get; init; } = string.Empty;
    public string Level3 { get; init; } = string.Empty;
    public string Level4 { get; init; } = string.Empty;
    public string Level5 { get; init; } = string.Empty;
    public Guid MonitoringNgoId { get; init; }
    public string NgoName { get; init; } = string.Empty;
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; } = null!;

    /// <summary>The follow-up status of the NGO that owns the record (see /follow-up-statuses); FollowUpStatus is the record's fixed value.</summary>
    public Guid? FollowUpStatusId { get; init; }
    public string? FollowUpStatusName { get; init; }
    public string? FollowUpStatusColor { get; init; }
    public string? FollowUpStatusIcon { get; init; }
}
