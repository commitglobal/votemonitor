using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Authorization.Policies.Specifications;

internal sealed class GetNgoStaffSpecification : SingleResultSpecification<NgoStaff, NgoStaffView>
{
    public GetNgoStaffSpecification(Guid ngoId, Guid staffId)
    {
        Query
            .Where(x => x.Id == staffId && x.NgoId == ngoId)
            .Include(x => x.Ngo)
            .AsNoTracking();

        Query.Select(x => new NgoStaffView
        {
            NgoId = x.Ngo.Id,
            NgoStatus = x.Ngo.Status,
            NgoStaffId = x.Id,
            UserStatus = x.ApplicationUser.Status
        });
    }
}
