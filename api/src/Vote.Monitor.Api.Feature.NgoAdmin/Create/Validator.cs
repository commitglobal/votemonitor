namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
             .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(3)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(256);
    }
}
