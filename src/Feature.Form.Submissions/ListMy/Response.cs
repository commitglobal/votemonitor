namespace Feature.Form.Submissions.ListMy;

public record Response
{
    public required List<FormSubmissionModel> Submissions { get; init; }
}
