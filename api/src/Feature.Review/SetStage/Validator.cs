namespace Feature.Review.SetStage;

public class Validator : ReportRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.StageId).NotEmpty();
    }
}
