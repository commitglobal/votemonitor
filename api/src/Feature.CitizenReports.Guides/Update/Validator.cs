using Vote.Monitor.Core.Validators;

namespace Feature.CitizenReports.Guides.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);

        RuleFor(x => x.WebsiteUrl)
            .IsValidUri()
            .MaximumLength(2048)
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));
    }
}