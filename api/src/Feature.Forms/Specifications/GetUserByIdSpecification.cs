using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetUserByIdSpecification : SingleResultSpecification<ApplicationUser>
{
    public GetUserByIdSpecification(Guid userId)
    {
        Query.Where(x => x.Id == userId);
    }
}
