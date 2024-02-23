namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId)
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
