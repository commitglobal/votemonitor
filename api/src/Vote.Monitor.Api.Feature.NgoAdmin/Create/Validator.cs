namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.LastName)
             .MinimumLength(3)
             .NotEmpty();

        RuleFor(x => x.Email)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.Password)
            .MinimumLength(3)
            .NotEmpty();
    }
}
