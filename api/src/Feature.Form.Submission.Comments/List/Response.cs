namespace Feature.Form.Submission.Comments.List;

public record Response
{
    public required List<FormSubmissionCommentModel> Comments { get; init; }
}
