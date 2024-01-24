using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public class GetNgoStatusSpecification : SingleResultSpecification<NgoAggregate, CSOStatus>
{
    public GetNgoStatusSpecification(Guid ngoId)
    {
        Query
            .Where(x => x.Id == ngoId);

        Query.Select(x => x.Status);
    }
}
