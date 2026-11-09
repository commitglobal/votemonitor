using Feature.SavedViews.Models;

namespace Feature.SavedViews.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<SavedViewsModel>, NotFound>>
{
    private static readonly StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/views");
        DontAutoTag();
        Options(x => x.WithTags("saved-views"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Lists the caller's NGO's saved views of a report list";
            s.Description =
                "The NGO's shared views and the caller's private ones, shared first, each A to Z ignoring case. " +
                "Also in archived rounds; citizen report views need the round's citizen-reporting NGO (otherwise empty).";
        });
    }

    public override async Task<Results<Ok<SavedViewsModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var member = await authorizationService.AuthorizeAsync(User, null,
            new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!member.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, req.RecordType, ct);
        if (monitoringNgoId is null)
        {
            return TypedResults.Ok(new SavedViewsModel([]));
        }

        var views = await context.SavedViews
            .AsNoTracking()
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringNgoId == monitoringNgoId
                        && x.RecordType == req.RecordType
                        && (x.Access == SavedViewAccess.Ngo || x.CreatedBy == req.UserId))
            .ToListAsync(ct);

        var ordered = views
            .OrderBy(x => x.Access == SavedViewAccess.Ngo ? 0 : 1)
            .ThenBy(x => x.Name, IgnoreCase)
            .ThenBy(x => x.Name, StringComparer.Ordinal)
            .ThenBy(x => x.CreatedOn)
            .ToList();

        var models = await context.ToModelsAsync(ordered, req.UserId, roleProvider.IsNgoAdmin(), ct);
        return TypedResults.Ok(new SavedViewsModel(models));
    }
}
