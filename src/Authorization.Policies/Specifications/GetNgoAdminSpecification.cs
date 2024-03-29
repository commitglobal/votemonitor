using Ardalis.Specification;

namespace Authorization.Policies.Specifications;

internal sealed class GetNgoAdminSpecification : SingleResultSpecification<NgoAdmin, NgoAdminView>
{
    public GetNgoAdminSpecification(Guid ngoId, Guid adminId)
    {
        Query
            .Where(x => x.Id == adminId && x.NgoId == ngoId)
            .Include(x => x.Ngo);

        Query.Select(x => new NgoAdminView
        {
            NgoId = x.Ngo.Id,
            NgoStatus = x.Ngo.Status,
            NgoAdminId = x.Id,
            UserStatus = x.Status
        });
    }
}
