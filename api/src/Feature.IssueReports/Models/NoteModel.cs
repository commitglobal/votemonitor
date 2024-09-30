using Vote.Monitor.Domain.Entities.IssueReportNoteAggregate;

namespace Feature.IssueReports.Models;

public record NoteModel
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public static NoteModel FromEntity(IssueReportNote note)
    {
        return note == null
            ? null
            : new NoteModel
            {
                SubmissionId = note.IssueReportId,
                QuestionId = note.QuestionId,
                Text = note.Text,
                TimeSubmitted = note.LastModifiedOn ?? note.CreatedOn
            };
    }
}