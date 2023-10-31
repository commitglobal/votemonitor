using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Country.Get;

public class Endpoint : Endpoint<Request, Results<Ok<CountryModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/countries/{id:guid}");
    }

    public override Task<Results<Ok<CountryModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var country = CountriesList.GetAll().FirstOrDefault(x => x.Id == req.Id);
        if (country is null)
        {
            return Task.FromResult<Results<Ok<CountryModel>, NotFound>>(TypedResults.NotFound());
        }

        var countryModel = new CountryModel
        {
            Id = country.Id,
            FullName = country.FullName,
            Name = country.Name,
            Iso2 = country.Iso2,
            Iso3 = country.Iso3,
            NumericCode = country.NumericCode
        };

        return Task.FromResult<Results<Ok<CountryModel>, NotFound>>(TypedResults.Ok(countryModel));
    }
}
