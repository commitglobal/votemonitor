using Vote.Monitor.Core.Validators;

namespace Feature.ObserverGuide.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);

        RuleFor(x => x.Attachment)
            .NotEmpty()!
            .When(x => string.IsNullOrEmpty(x.WebsiteUrl)); // 50 MB upload limit

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()!
            .MaximumLength(2048)
            .When(x => x.Attachment == null);
    }
}
