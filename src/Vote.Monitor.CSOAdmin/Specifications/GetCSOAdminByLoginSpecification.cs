using Ardalis.Specification;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class GetCSOAdminByLoginSpecification : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public GetCSOAdminByLoginSpecification(Guid csoId, string login)
    {
        Query
            .Where(x => x.CSOId == csoId && x.Login == login);
    }
}
