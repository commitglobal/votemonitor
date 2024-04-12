namespace Vote.Monitor.Api.Feature.Observer.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);

    }
}
