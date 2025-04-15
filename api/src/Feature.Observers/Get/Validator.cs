namespace Feature.Observers.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
