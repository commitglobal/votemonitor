using Vote.Monitor.Domain.Entities.FormAggregate;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Models;

public record CitizenReportEntryModel
{
    public Guid CitizenReportId { get; set; }
    public DateTime TimeSubmitted { get; set; }
    public string FormCode { get; set; }
    public FormStatus FormStatus { get; set; } = null!;

    public TranslatedString FormName { get; set; }

    public string FormDefaultLanguage { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NotesCount { get; set; }
    public int MediaFilesCount { get; set; }
    public int CommentsCount { get; set; }

    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }

    /// <summary>The record's review stage in the caller's NGO.</summary>
    public Guid StageId { get; init; }
    public string StageName { get; init; } = "";
    public string ReviewStatus { get; init; } = "";
    public DateTime? StageChangedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }

    /// <summary>The record's reviewers in the caller's NGO.</summary>
    public Guid[] ReviewerIds { get; init; } = [];

    /// <summary>The record's severity in the caller's NGO: None, Low, Medium, High or Urgent.</summary>
    public string Severity { get; init; } = "None";
}