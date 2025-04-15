namespace Feature.Auth.SetUserPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.AspNetUserId)
            .NotEmpty();

        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}
