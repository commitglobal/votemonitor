namespace Vote.Monitor.Api.Feature.Auth.RefreshToken;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
