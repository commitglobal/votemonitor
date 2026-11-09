using Feature.SavedViews.Models;

namespace Feature.SavedViews.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<SavedViewModel>, NotFound, ForbidHttpResult>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/views/{viewId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("saved-views"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Replaces a view's name, description, access and state";
            s.Description =
                "Private views: their creator. Shared views: their creator or any NGO admin. Only the creator can " +
                "change who sees a view. A view the caller doesn't see is 404; one they see but can't change is 403.";
        });
    }

    public override async Task<Results<Ok<SavedViewModel>, NotFound, ForbidHttpResult>> ExecuteAsync(Request req,
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

        var isNgoAdmin = roleProvider.IsNgoAdmin();
        var access = req.Access ?? SavedViewAccess.Ngo;
        if (!SavedViewAccessRules.CanEdit(view, req.UserId, isNgoAdmin)
            || (access != view.Access && !SavedViewAccessRules.CanChangeAccess(view, req.UserId)))
        {
            return TypedResults.Forbid();
        }

        view.Update(req.Name.Trim(),
            SavedViewState.Description(req.Description),
            access,
            SavedViewState.ToDocument(req.State!.Value));
        await context.SaveChangesAsync(ct);

        var model = (await context.ToModelsAsync([view], req.UserId, isNgoAdmin, ct)).Single();
        return TypedResults.Ok(model);
    }
}
