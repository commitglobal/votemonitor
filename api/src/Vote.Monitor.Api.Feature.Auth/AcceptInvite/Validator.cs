namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(p => p.InviteCode)
            .NotEmpty();

        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(p => p.NewPassword)
            .NotEmpty();

        RuleFor(p => p.ConfirmNewPassword)
            .Equal(p => p.NewPassword)
            .WithMessage("Passwords must match");
    }
}
