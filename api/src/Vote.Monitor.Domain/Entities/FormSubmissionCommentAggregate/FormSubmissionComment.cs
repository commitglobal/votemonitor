using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Domain.Entities.FormSubmissionCommentAggregate;

public class FormSubmissionComment : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid SubmissionId { get; private set; }
    public FormSubmission Submission { get; private set; }
    public Guid? QuestionId { get; private set; }
    public string Text { get; private set; }

    private FormSubmissionComment(Guid id,
        Guid electionRoundId,
        Guid submissionId,
        Guid? questionId,
        string text)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        SubmissionId = submissionId;
        QuestionId = questionId;
        Text = text;
    }

    public static FormSubmissionComment Create(Guid electionRoundId,
        Guid submissionId,
        Guid? questionId,
        string text) =>
        new(Guid.NewGuid(), electionRoundId, submissionId, questionId, text);

    public void UpdateText(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal FormSubmissionComment()
    {
    }
#pragma warning restore CS8618
}
