using Ardalis.Specification;
using Feature.DataExport.Details;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.DataExport.Specifications;

public sealed class GetElectionRoundSpecification : SingleResultSpecification<ElectionRound, Export.Response.ElectionRoundModel>
{
    public GetElectionRoundSpecification(Guid electionRoundId)
    {
        Query.Where(x => x.Id ==electionRoundId);

        Query.Select(x => new Export.Response.ElectionRoundModel
        {
            Id = x.Id,
            Title  = x.Title,
            EnglishTitle = x.EnglishTitle,
            StartDate = x.StartDate,
        });
    }
}
