using Ardalis.Specification;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class GetCSOAdminByIdSpecification : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>, ISingleResultSpecification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public GetCSOAdminByIdSpecification(Guid csoId, Guid adminId)
    {
        Query
            .Where(x => x.CSOId == csoId && x.Id == adminId);
    }
}
