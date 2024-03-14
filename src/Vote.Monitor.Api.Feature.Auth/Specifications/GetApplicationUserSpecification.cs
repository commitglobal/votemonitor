using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.Auth.Specifications;

public sealed class GetApplicationUserSpecification : Specification<ApplicationUser>, ISingleResultSpecification<ApplicationUser>
{
    public GetApplicationUserSpecification(string username, string password)
    {
        Query
            .Where(x => x.Login == username && x.Password == password);
    }
}
