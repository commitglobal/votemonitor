using Feature.SavedViews.Models;

namespace Feature.SavedViews.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Created<SavedViewModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/views");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("saved-views"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Saves a report list's state as a view of the caller's NGO";
            s.Description =
                "NGO admins and staff, also in archived rounds. The state is stored as given and returned unchanged. " +
                $"An NGO can have at most {SavedView.MaxViewsPerRecordType} views per round and record type.";
        });
    }

    public override async Task<Results<Created<SavedViewModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, req.RecordType, ct);
        var creatorName = await context.Users
            .Where(x => x.Id == req.UserId)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(ct);
        if (monitoringNgoId is null || creatorName is null)
        {
            return TypedResults.NotFound();
        }

        var count = await context.SavedViews.CountAsync(x => x.ElectionRoundId == req.ElectionRoundId
                                                             && x.MonitoringNgoId == monitoringNgoId
                                                             && x.RecordType == req.RecordType, ct);
        if (count >= SavedView.MaxViewsPerRecordType)
        {
            AddError(x => x.Name,
                $"Your organization can have at most {SavedView.MaxViewsPerRecordType} views of this list");
        }

        ThrowIfAnyErrors();

        var view = SavedView.Create(req.ElectionRoundId,
            monitoringNgoId.Value,
            req.RecordType,
            req.Name.Trim(),
            SavedViewState.Description(req.Description),
            req.Access ?? SavedViewAccess.Ngo,
            SavedViewState.ToDocument(req.State!.Value),
            req.UserId,
            creatorName);
        context.SavedViews.Add(view);
        await context.SaveChangesAsync(ct);

        var model = (await context.ToModelsAsync([view], req.UserId, roleProvider.IsNgoAdmin(), ct)).Single();
        return TypedResults.Created($"/api/election-rounds/{req.ElectionRoundId}/views/{view.Id}", model);
    }
}
