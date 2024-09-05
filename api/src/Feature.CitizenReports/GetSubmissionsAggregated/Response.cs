using Feature.CitizenReports.Models;
using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.CitizenReports.GetSubmissionsAggregated;

public class Response
{
    public CitizenReportFormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public AttachmentModel[] Attachments { get; set; } = [];
    public NoteModel[] Notes { get; set; } = [];
}