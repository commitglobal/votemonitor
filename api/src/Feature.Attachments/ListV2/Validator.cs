namespace Feature.Attachments.ListV2;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.SubmissionId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
    }
}
