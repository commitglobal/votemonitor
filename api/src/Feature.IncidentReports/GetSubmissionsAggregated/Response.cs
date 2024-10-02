using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.IncidentReports.GetSubmissionsAggregated;

public class Response
{
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public AttachmentModel[] Attachments { get; set; } = [];
    public NoteModel[] Notes { get; set; } = [];
}