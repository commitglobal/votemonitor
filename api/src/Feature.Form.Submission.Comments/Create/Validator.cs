namespace Feature.Form.Submission.Comments.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.SubmissionId).NotEmpty();
        RuleFor(x => x.QuestionId)
            .Must(id => id != Guid.Empty)
            .When(x => x.QuestionId.HasValue);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(10_000);
    }
}
