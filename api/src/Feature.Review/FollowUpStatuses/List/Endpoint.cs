namespace Feature.Review.FollowUpStatuses.List;

public class Request : FollowUpStatusesRequest;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
    }
}

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<FollowUpStatusModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/follow-up-statuses");
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's follow-up statuses in the round, in order"; });
    }

    public override async Task<Results<Ok<FollowUpStatusModel[]>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var statuses = await context.FollowUpStatuses
            .AsNoTracking()
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId && x.MonitoringNgo.NgoId == req.NgoId)
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);

        return TypedResults.Ok(statuses.Select(FollowUpStatusModel.FromEntity).ToArray());
    }
}
