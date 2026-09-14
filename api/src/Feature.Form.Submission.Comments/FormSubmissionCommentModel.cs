namespace Feature.Form.Submission.Comments;

public record FormSubmissionCommentModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid SubmissionId { get; init; }
    public Guid? QuestionId { get; init; }
    public required string Text { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string CreatedByName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public static FormSubmissionCommentModel FromEntity(FormSubmissionCommentAggregate comment)
        => new()
        {
            Id = comment.Id,
            ElectionRoundId = comment.ElectionRoundId,
            SubmissionId = comment.SubmissionId,
            QuestionId = comment.QuestionId,
            Text = comment.Text,
            CreatedBy = comment.CreatedBy,
            CreatedByName = string.Empty,
            CreatedAt = comment.CreatedOn,
            LastModifiedAt = comment.LastModifiedOn
        };
}
