namespace Feature.SavedViews.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound, ForbidHttpResult>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/views/{viewId}");
        DontAutoTag();
        Options(x => x.WithTags("saved-views"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Deletes one of the caller's NGO's views";
            s.Description =
                "Private views: their creator. Shared views: their creator or any NGO admin. A view the caller " +
                "doesn't see is 404; one they see but can't delete is 403.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ForbidHttpResult>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var view = await context.SavedViews.FirstOrDefaultAsync(x => x.Id == req.ViewId
                                                                     && x.ElectionRoundId == req.ElectionRoundId, ct);
        if (view is null || !SavedViewAccessRules.CanSee(view, req.UserId))
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, view.RecordType, ct);
        if (monitoringNgoId != view.MonitoringNgoId)
        {
            return TypedResults.NotFound();
        }

        if (!SavedViewAccessRules.CanEdit(view, req.UserId, roleProvider.IsNgoAdmin()))
        {
            return TypedResults.Forbid();
        }

        context.SavedViews.Remove(view);
        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
