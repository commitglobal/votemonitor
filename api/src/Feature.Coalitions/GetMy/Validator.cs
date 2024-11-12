namespace Feature.NgoCoalitions.GetMy;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId).NotEmpty();
    }
}
