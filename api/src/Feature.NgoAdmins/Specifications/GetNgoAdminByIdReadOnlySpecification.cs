namespace Feature.NgoAdmins.Specifications;

public sealed class GetNgoAdminByIdReadOnlySpecification : Specification<NgoAdminAggregate>, ISingleResultSpecification<NgoAdminAggregate>
{
    public GetNgoAdminByIdReadOnlySpecification(Guid ngoId, Guid adminId)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.NgoId == ngoId && x.Id == adminId)
            .Include(x=>x.ApplicationUser);
    }
}
