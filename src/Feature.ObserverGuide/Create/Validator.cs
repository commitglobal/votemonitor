using Vote.Monitor.Core.Validators;

namespace Feature.ObserverGuide.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Attachment).FileSmallerThan(50 * 1024 * 1024); // 50 MB upload limit
    }
}
