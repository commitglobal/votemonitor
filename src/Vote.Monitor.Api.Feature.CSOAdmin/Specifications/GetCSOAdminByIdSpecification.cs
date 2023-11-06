namespace Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

public class GetCSOAdminByIdSpecification : Specification<CSOAdminAggregate>, ISingleResultSpecification<CSOAdminAggregate>
{
    public GetCSOAdminByIdSpecification(Guid csoId, Guid adminId)
    {
        Query
            .Where(x => x.CSOId == csoId && x.Id == adminId);
    }
}
