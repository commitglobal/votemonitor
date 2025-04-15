namespace Feature.Countries.List;

public class Endpoint : EndpointWithoutRequest<List<CountryModel>>
{

    public override void Configure()
    {
        Get("/api/countries");
    }

    public override Task<List<CountryModel>> ExecuteAsync(CancellationToken ct)
    {
        var countries = CountriesList
            .GetAll()
            .Select(country => new CountryModel
            {
                Id = country.Id,
                FullName = country.FullName,
                Name = country.Name,
                Iso2 = country.Iso2,
                Iso3 = country.Iso3,
                NumericCode = country.NumericCode
            }).ToList();

        return Task.FromResult(countries);
    }
}
