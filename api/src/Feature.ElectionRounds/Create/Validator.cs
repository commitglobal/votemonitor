namespace Feature.ElectionRounds.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CountryId)
            .NotEmpty()
            .Must(CountriesList.IsKnownCountry)
            .WithMessage("Unknown country id.");

        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(256)
            .NotEmpty();

        RuleFor(x => x.EnglishTitle)
            .MinimumLength(3)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.EnglishTitle));

        // Today or later where the admin is: the date is a plain day, so today counts in any time zone (UTC-12 is at
        // most one day behind UTC).
        RuleFor(x => x.StartDate)
            .Must(startDate =>
            {
                var timeProvider = Resolve<ITimeProvider>();
                return startDate >= timeProvider.UtcNowDate.AddDays(-1);
            })
            .WithMessage("Election start date must be today or later.");
    }
}
