using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public sealed class ListElectionRoundsSpecification : Specification<ElectionRoundAggregate, ElectionRoundBaseModel>
{
    public ListElectionRoundsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Title, "%" + request.TitleFilter + "%", !string.IsNullOrEmpty(request.TitleFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .Where(x => x.CountryId == request.CountryId, request.CountryId != null)
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new ElectionRoundBaseModel
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
