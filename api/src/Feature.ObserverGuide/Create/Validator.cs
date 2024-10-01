using Vote.Monitor.Core.Validators;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);

        RuleFor(x => x.Attachment)
            .NotEmpty()
            .FileSmallerThan(50 * 1024 * 1024) // 50 MB upload limit
            .When(x => x.GuideType == ObserverGuideType.Document);

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .MaximumLength(2048)
            .IsValidUri()
            .When(x => x.GuideType == ObserverGuideType.Website);

        RuleFor(x => x.Text)
            .NotEmpty()
            .When(x => x.GuideType == ObserverGuideType.Text);
    }
}