using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-guides/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-guides"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        await context
            .CitizenGuides
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.Id &&
                        x.GuideType != CitizenGuideType.Document)
            .ExecuteDeleteAsync(ct);

        await context
            .CitizenGuides
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(g => g.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}