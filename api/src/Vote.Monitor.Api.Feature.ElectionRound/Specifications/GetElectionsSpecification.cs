namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetElectionsSpecification : Specification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetElectionsSpecification()
    {
        Query.Where(x => x.Status != ElectionRoundStatus.Archived);

        Query.Select(x => new ElectionRoundModel
        {
            Id = x.Id,
            Title = x.Title,
            EnglishTitle = x.EnglishTitle,
            StartDate = x.StartDate,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
            CountryIso2 = x.Country.Iso2,
            CountryIso3 = x.Country.Iso3,
            CountryName = x.Country.Name,
            CountryFullName = x.Country.FullName,
            CountryNumericCode = x.Country.NumericCode,
            CountryId = x.CountryId
        });
    }
}
