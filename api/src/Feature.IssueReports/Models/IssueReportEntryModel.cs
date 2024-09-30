using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.Models;

public record IssueReportEntryModel
{
    public Guid IssueReportId { get; set; }
    public DateTime TimeSubmitted { get; set; }
    public string FormCode { get; set; }

    public TranslatedString FormName { get; set; }

    public string FormDefaultLanguage { get; set; }
    public string ObserverName { get; set; }
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string[] Tags { get; set; } = [];
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NotesCount { get; set; }
    public int MediaFilesCount { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportLocationType, string>))]
    public IssueReportLocationType LocationType { get; set; }

    public string? LocationDescription { get; set; }
    public string? Level1 { get; set; }
    public string? Level2 { get; set; }
    public string? Level3 { get; set; }
    public string? Level4 { get; set; }
    public string? Level5 { get; set; }
    public string? PollingStationNumber { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IssueReportFollowUpStatus, string>))]
    public IssueReportFollowUpStatus FollowUpStatus { get; set; }
}