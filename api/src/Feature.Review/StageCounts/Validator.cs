namespace Feature.Review.StageCounts;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.RecordType).NotNull();
        RuleFor(x => x.DataSource).NotNull();
    }
}
