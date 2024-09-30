using Feature.IssueReports.Models;
using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.IssueReports.GetSubmissionsAggregated;

public class Response
{
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public AttachmentModel[] Attachments { get; set; } = [];
    public NoteModel[] Notes { get; set; } = [];
}