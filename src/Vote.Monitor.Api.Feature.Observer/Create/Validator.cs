namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.Password)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
           .NotEmpty()
            .MinimumLength(3)
           .MaximumLength(32);
    }
}
