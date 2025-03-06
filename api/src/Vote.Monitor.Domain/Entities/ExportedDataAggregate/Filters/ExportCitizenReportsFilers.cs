using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

public record ExportCitizenReportsFilers
{
    public Guid NgoId { get; init; }
    public string? SearchText { get; init; }
    public string? Level1Filter { get; init; }
    public string? Level2Filter { get; init; }
    public string? Level3Filter { get; init; }
    public string? Level4Filter { get; init; }
    public string? Level5Filter { get; init; }
    public bool? HasFlaggedAnswers { get; init; }

    public CitizenReportFollowUpStatus? FollowUpStatus { get; init; }

    public Guid? FormId { get; init; }
    public bool? HasNotes { get; init; }
    public bool? HasAttachments { get; init; }

    public QuestionsAnsweredFilter? QuestionsAnswered { get; init; }

    public DateTime? FromDateFilter { get; init; }
    public DateTime? ToDateFilter { get; init; }
}
