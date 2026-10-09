namespace Feature.NgoStaff.Specifications;

public sealed class GetNgoStaffByIdSpecification : Specification<NgoStaffAggregate>, ISingleResultSpecification<NgoStaffAggregate>
{
    public GetNgoStaffByIdSpecification(Guid ngoId, Guid adminId)
    {
        Query
            .Where(x => x.NgoId == ngoId && x.Id == adminId)
            .Include(x=>x.ApplicationUser);
    }
}
