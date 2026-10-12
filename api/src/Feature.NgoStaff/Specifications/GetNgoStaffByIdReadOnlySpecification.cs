namespace Feature.NgoStaff.Specifications;

public sealed class GetNgoStaffByIdReadOnlySpecification : Specification<NgoStaffAggregate>, ISingleResultSpecification<NgoStaffAggregate>
{
    public GetNgoStaffByIdReadOnlySpecification(Guid ngoId, Guid adminId)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.NgoId == ngoId && x.Id == adminId)
            .Include(x=>x.ApplicationUser);
    }
}
