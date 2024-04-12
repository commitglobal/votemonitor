namespace Vote.Monitor.Api.Feature.Auth.ResetPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
