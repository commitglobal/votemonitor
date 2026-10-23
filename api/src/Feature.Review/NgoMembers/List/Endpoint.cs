using Feature.Review.Models;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Feature.Review.NgoMembers.List;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<NgoMemberModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/ngo-members");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's admins and staff, the people who can review records"; });
    }

    public override async Task<Results<Ok<NgoMemberModel[]>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var admins = await context.NgoAdmins
            .Where(x => x.NgoId == req.NgoId)
            .Where(x => x.ApplicationUser.Status == UserStatus.Active)
            .Select(x => new
            {
                Id = x.ApplicationUserId,
                DisplayName = x.ApplicationUser.DisplayName,
                Email = x.ApplicationUser.Email!,
            })
            .ToListAsync(ct);
        var staff = await context.NgoStaff
            .Where(x => x.NgoId == req.NgoId)
            .Where(x => x.ApplicationUser.Status == UserStatus.Active)
            .Select(x => new
            {
                Id = x.ApplicationUserId,
                DisplayName = x.ApplicationUser.DisplayName,
                Email = x.ApplicationUser.Email!,
            })
            .ToListAsync(ct);

        return TypedResults.Ok(admins
            .Select(x =>
                new NgoMemberModel
                {
                    Id = x.Id, DisplayName = x.DisplayName, Email = x.Email!, Role = UserRole.NgoAdmin
                }).Concat(staff.Select(x => new NgoMemberModel
            {
                Id = x.Id, DisplayName = x.DisplayName, Email = x.Email!, Role = UserRole.NgoStaff
            })).OrderBy(x => x.DisplayName).ToArray());
    }
}
