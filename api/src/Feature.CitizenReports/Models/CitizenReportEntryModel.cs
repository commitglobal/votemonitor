using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Models;

public record CitizenReportEntryModel
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public string FormCode { get; init; } = default!;
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }
    public int MediaFilesCount { get; init; }
    public int NotesCount { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }
}

