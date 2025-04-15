namespace Feature.PollingStation.Information.Forms.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
