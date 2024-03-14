namespace Vote.Monitor.Api.Feature.Notifications.Send;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.ObserverIds).NotEmpty();
        RuleForEach(x => x.ObserverIds).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(1024);
    }
}

