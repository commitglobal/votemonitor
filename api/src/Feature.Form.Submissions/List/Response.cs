namespace Feature.Form.Submissions.List;

public record Response
{
    public required List<FormSubmissionModel> Submissions { get; init; }
}
