using Feature.Form.Submissions.Models;
using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.Form.Submissions.GetAggregated;

public class Response
{
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public List<AttachmentModel> Attachments { get; set; }
    public List<NoteModel> Notes { get; set; }
}
