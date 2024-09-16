using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;

namespace Feature.CitizenReports.Models;

public record NoteModel
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public static NoteModel FromEntity(CitizenReportNote note)
    {
        return note == null
            ? null
            : new NoteModel
            {
                SubmissionId = note.CitizenReportId,
                QuestionId = note.QuestionId,
                Text = note.Text,
                TimeSubmitted = note.LastModifiedOn ?? note.CreatedOn
            };
    }
}