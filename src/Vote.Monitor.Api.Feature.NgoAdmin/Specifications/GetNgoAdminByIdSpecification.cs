namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public class GetNgoAdminByIdSpecification : Specification<NgoAdminAggregate>, ISingleResultSpecification<NgoAdminAggregate>
{
    public GetNgoAdminByIdSpecification(Guid ngoId, Guid adminId)
    {
        Query
            .Where(x => x.NgoId == ngoId && x.Id == adminId);
    }
}
