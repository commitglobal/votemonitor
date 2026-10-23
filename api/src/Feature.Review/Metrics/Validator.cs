namespace Feature.Review.Metrics;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.DataSource).NotNull();
        RuleFor(x => x.Level).InclusiveBetween(1, 5).When(x => x.Level is not null);
    }
}
