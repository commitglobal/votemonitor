using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.UserPreferences.Specifications;

public sealed class GetUserByIdReadOnlySpecification : SingleResultSpecification<ApplicationUser>
{
    public GetUserByIdReadOnlySpecification(Guid userId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.Id == userId);
    }
}
