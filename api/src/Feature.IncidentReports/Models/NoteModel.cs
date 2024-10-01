using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

namespace Feature.IncidentReports.Models;

public record NoteModel
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
    public DateTime TimeSubmitted { get; init; }

    public static NoteModel FromEntity(IncidentReportNote note)
    {
        return note == null
            ? null
            : new NoteModel
            {
                SubmissionId = note.IncidentReportId,
                QuestionId = note.QuestionId,
                Text = note.Text,
                TimeSubmitted = note.LastModifiedOn ?? note.CreatedOn
            };
    }
}