namespace Feature.NgoAdmins.Specifications;

public sealed class GetNgoAdminByIdSpecification : Specification<NgoAdminAggregate>, ISingleResultSpecification<NgoAdminAggregate>
{
    public GetNgoAdminByIdSpecification(Guid ngoId, Guid adminId)
    {
        Query
            .Where(x => x.NgoId == ngoId && x.Id == adminId)
            .Include(x=>x.ApplicationUser);
    }
}
