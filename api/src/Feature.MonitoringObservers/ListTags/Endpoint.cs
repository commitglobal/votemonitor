using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.MonitoringObservers.ListTags;

public class Endpoint(IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers:tags");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(request.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var tags = await context
            .MonitoringObservers
            .Where(x=>x.ElectionRoundId == request.ElectionRoundId)
            .Where(x => x.MonitoringNgo.NgoId == request.NgoId
                        && x.MonitoringNgo.ElectionRoundId == request.ElectionRoundId)
            .Where(x => x.Tags.Any())
            .Select(x => Postgres.Functions.Unnest(x.Tags))
            .Distinct()
            .ToListAsync(cancellationToken: ct);

        return TypedResults.Ok(new Response
        {
            Tags = tags.ToList().AsReadOnly()
        });
    }
}
