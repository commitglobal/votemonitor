using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Feature.DataExport.Start;

public record CitizenReportsFilters
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

    public ExportCitizenReportsFilers ToFilter()
    {
        return new ExportCitizenReportsFilers
        {
            NgoId = NgoId,
            SearchText = SearchText,
            Level1Filter = Level1Filter,
            Level2Filter = Level2Filter,
            Level3Filter = Level3Filter,
            Level4Filter = Level4Filter,
            Level5Filter = Level5Filter,
            HasFlaggedAnswers = HasFlaggedAnswers,
            FollowUpStatus = FollowUpStatus,
            FormId = FormId,
            HasNotes = HasNotes,
            HasAttachments = HasAttachments,
            QuestionsAnswered = QuestionsAnswered,
            FromDateFilter = FromDateFilter,
            ToDateFilter = ToDateFilter
        };
    }
}
