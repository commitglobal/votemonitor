using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public class GetNgoStatusSpecification : SingleResultSpecification<NgoAggregate, NgoStatus>
{
    public GetNgoStatusSpecification(Guid ngoId)
    {
        Query
            .Where(x => x.Id == ngoId);

        Query.Select(x => x.Status);
    }
}
