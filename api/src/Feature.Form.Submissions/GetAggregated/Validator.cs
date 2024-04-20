namespace Feature.Form.Submissions.GetAggregated;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
    }
}
