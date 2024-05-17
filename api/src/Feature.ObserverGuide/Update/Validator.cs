namespace Feature.ObserverGuide.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
    }
}
