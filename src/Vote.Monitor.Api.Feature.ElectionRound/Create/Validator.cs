namespace Vote.Monitor.Api.Feature.ElectionRound.Create;

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
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .Must(startDate =>
            {
                var timeProvider = Resolve<ITimeProvider>();
                return startDate > timeProvider.UtcNowDate;
            })
            .WithMessage("Election start date must be in the future.");
    }
}
