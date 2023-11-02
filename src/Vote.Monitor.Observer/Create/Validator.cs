namespace Vote.Monitor.Observer.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.Login)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.Password)
            .MinimumLength(3)
            .NotEmpty();
    }
}
