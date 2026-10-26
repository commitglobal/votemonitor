namespace Feature.ElectionRounds.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleForEach(x => x.CountryId)
            .Must(countryId => CountriesList.IsKnownCountry(countryId))
            .WithMessage("Unknown country id.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
