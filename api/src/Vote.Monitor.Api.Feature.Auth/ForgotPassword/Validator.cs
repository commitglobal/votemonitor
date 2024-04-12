namespace Vote.Monitor.Api.Feature.Auth.ForgotPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
