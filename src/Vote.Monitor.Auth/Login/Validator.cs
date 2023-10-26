using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Auth.Login;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
