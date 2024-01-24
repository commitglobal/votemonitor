namespace Vote.Monitor.Api.Feature.ElectionRound.Archive;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
