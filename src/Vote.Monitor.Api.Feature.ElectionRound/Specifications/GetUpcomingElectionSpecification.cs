namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class GetUpcomingElectionSpecification : Specification<ElectionRoundAggregate, ElectionRoundModel>
{
    public GetUpcomingElectionSpecification(ITimeProvider timeProvider)
    {
        var today = timeProvider.UtcNowDate;

        Query.Where(x => x.StartDate >= today);

        Query.Select(x => new ElectionRoundModel
        {
            Id = x.Id,
            Title = x.Title,
            EnglishTitle = x.EnglishTitle,
            StartDate = x.StartDate,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
            Country = x.Country.FullName,
            CountryId = x.CountryId
        });
    }
}
