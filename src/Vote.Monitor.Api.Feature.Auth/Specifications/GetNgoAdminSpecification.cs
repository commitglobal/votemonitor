using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.Auth.Specifications;

public sealed class GetNgoAdminSpecification : SingleResultSpecification<NgoAdmin>
{
    public GetNgoAdminSpecification(Guid ngoAdminId)
    {
        Query.Where(x => x.Id == ngoAdminId);
    }
}
