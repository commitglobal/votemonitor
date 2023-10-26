using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Auth.Login;

public class GetApplicationUserSpecification : Specification<ApplicationUser>, ISingleResultSpecification<ApplicationUser>
{
    public GetApplicationUserSpecification(string username, string password)
    {
        Query
            .Where(x => x.Login == username && x.Password == password);
    }
}
