namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CountryId)
            .Must(countryId => CountriesList.IsKnownCountry(countryId!.Value))
            .When(x => x.CountryId != null)
            .WithMessage("Unknown country id.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
