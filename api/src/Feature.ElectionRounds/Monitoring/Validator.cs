namespace Feature.ElectionRounds.Monitoring;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId).NotEmpty();
    }
}
