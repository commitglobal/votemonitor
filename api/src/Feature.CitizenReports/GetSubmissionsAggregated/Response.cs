using Module.Answers.Aggregators;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using AttachmentModel = Feature.CitizenReports.Models.AttachmentModel;
using NoteModel = Feature.CitizenReports.Models.NoteModel;

namespace Feature.CitizenReports.GetSubmissionsAggregated;

public class Response
{
    public CitizenReportFormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public AttachmentModel[] Attachments { get; set; } = [];
    public NoteModel[] Notes { get; set; } = [];

    public SubmissionsFilterModel SubmissionsFilter { get; set; }
}

public class SubmissionsFilterModel
{
    public string? Level1Filter { get; set; }

    public string? Level2Filter { get; set; }

    public string? Level3Filter { get; set; }

    public string? Level4Filter { get; set; }

    public string? Level5Filter { get; set; }

    public bool? HasFlaggedAnswers { get; set; }

    public CitizenReportFollowUpStatus? FollowUpStatus { get; set; }

    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
}
