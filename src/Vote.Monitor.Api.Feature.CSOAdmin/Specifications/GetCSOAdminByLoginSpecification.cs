namespace Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

public class GetCSOAdminByLoginSpecification : Specification<CSOAdminAggregate>
{
    public GetCSOAdminByLoginSpecification(Guid csoId, string login)
    {
        Query
            .Where(x => x.CSOId == csoId && x.Login == login);
    }
}
