using Ardalis.Specification;

namespace Vote.Monitor.Auth.Specifications;

public class GetApplicationUserSpecification : Specification<ApplicationUser>, ISingleResultSpecification<ApplicationUser>
{
    public GetApplicationUserSpecification(string username, string password)
    {
        Query
            .Where(x => x.Login == username && x.Password == password);
    }
}
