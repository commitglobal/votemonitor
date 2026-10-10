namespace Feature.Form.Submissions.GetAggregatedV2;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.DataSource).NotEmpty();
    }
}
