using System.Xml.Linq;
using Ardalis.Specification;

namespace Vote.Monitor.CSOAdmin.Specifications;

public class GetCSOAdminByName : Specification<Domain.Entities.ApplicationUserAggregate.CSOAdmin>
{
    public GetCSOAdminByName(Guid CSOId, string name)
    {
        Query
            .Where(x => x.CSOId == CSOId && x.Name == name);
    }
}
