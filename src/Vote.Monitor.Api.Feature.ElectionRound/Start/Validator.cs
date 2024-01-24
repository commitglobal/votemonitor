namespace Vote.Monitor.Api.Feature.ElectionRound.Start;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
