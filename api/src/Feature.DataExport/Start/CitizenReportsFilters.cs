using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Feature.DataExport.Start;

public class CitizenReportsFilters
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

    public ExportCitizenReportsFilers ToFilter()
    {
        return new ExportCitizenReportsFilers
        {
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