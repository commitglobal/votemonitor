using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

public class ExportCitizenReportsFilers
{
    public string? SearchText { get; set; }
    public string? Level1Filter { get; set; }
    public string? Level2Filter { get; set; }
    public string? Level3Filter { get; set; }
    public string? Level4Filter { get; set; }
    public string? Level5Filter { get; set; }
    public bool? HasFlaggedAnswers { get; set; }

    public CitizenReportFollowUpStatus? FollowUpStatus { get; set; }

    public Guid? FormId { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }

    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }

    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }
}