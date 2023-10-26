using Ardalis.Specification;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class GetCSOAdmin : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public GetCSOAdmin(Guid CSOId, Guid adminId)
    {
        Query
            .Where(x => x.CSOId == CSOId && x.Id == adminId);
    }
}
