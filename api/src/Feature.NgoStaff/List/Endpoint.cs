using Authorization.Policies;
using Feature.NgoStaff.Specifications;
using Vote.Monitor.Core.Models;

namespace Feature.NgoStaff.List;

public class Endpoint(IReadRepository<NgoStaffAggregate> repository)
    : Endpoint<Request, Results<Ok<PagedResponse<NgoStaffModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/ngos/{ngoId}/staff");
        DontAutoTag();
        Options(x => x.WithTags("ngo-staff"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<NgoStaffModel>>, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new ListNgoStaffSpecification(req);
        var admins = await repository.ListAsync(specification, ct);
        var adminsCount = await repository.CountAsync(specification, ct);

        var result = admins.Select(x => new NgoStaffModel
        {
            Id = x.Id,
            FirstName = x.ApplicationUser.FirstName,
            LastName = x.ApplicationUser.LastName,
            Email = x.ApplicationUser.Email!,
            PhoneNumber = x.ApplicationUser.PhoneNumber,
            Status = x.ApplicationUser.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return TypedResults.Ok(new PagedResponse<NgoStaffModel>(result, adminsCount, req.PageNumber, req.PageSize));
    }
}
