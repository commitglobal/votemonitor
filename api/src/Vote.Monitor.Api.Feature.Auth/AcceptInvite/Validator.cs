namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(p => p.InvitationToken)
            .NotEmpty();

        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(p => p.ConfirmPassword)
            .Equal(p => p.Password)
            .WithMessage("Passwords must match");
    }
}
