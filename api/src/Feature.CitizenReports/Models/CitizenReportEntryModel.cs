using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Models;

public record CitizenReportEntryModel
{
    public Guid CitizenReportId { get; set; }
    public DateTime TimeSubmitted { get; set; }
    public string FormCode { get; set; }
    
    public TranslatedString FormName { get; set; }
    
    public string FormDefaultLanguage { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NotesCount { get; set; }
    public int MediaFilesCount { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportFollowUpStatus, string>))]
    public CitizenReportFollowUpStatus FollowUpStatus { get; init; }
}