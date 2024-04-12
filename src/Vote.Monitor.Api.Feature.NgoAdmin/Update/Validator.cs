namespace Vote.Monitor.Api.Feature.NgoAdmin.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MinimumLength(3);
    }
}
