using System.Security.Claims;
using Feature.Review;
using Feature.SavedViews.Models;

namespace Feature.SavedViews;

/// <summary>Who can see and change the NGO's views (docs/views-backend.md in the web app).</summary>
public static class SavedViewAccessRules
{
    /// <summary>
    /// The caller's monitoring NGO in the round when they are an admin or staff member of an active NGO that can work on
    /// this kind of report there (citizen reports: the round's citizen-reporting NGO), also in archived rounds.
    /// </summary>
    public static async Task<Guid?> ResolveMonitoringNgoIdAsync(this VoteMonitorContext context,
        IAuthorizationService authorizationService,
        ClaimsPrincipal user,
        Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(user, electionRoundId, recordType))
        {
            return null;
        }

        return await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
    }

    /// <summary>Whether the caller sees the view: the NGO's shared views and their own private ones.</summary>
    public static bool CanSee(SavedView view, Guid userId) =>
        view.Access == SavedViewAccess.Ngo || view.CreatedBy == userId;

    /// <summary>Private views: their creator. Shared views: their creator or any NGO admin.</summary>
    public static bool CanEdit(SavedView view, Guid userId, bool isNgoAdmin) =>
        view.CreatedBy == userId || (view.Access == SavedViewAccess.Ngo && isNgoAdmin);

    /// <summary>Only the creator changes who sees a view.</summary>
    public static bool CanChangeAccess(SavedView view, Guid userId) => view.CreatedBy == userId;

    /// <summary>
    /// The views as the caller sees them, each with whether they may change it; the creator's current name, or the one
    /// kept with the view when their account is gone.
    /// </summary>
    public static async Task<List<SavedViewModel>> ToModelsAsync(this VoteMonitorContext context,
        IReadOnlyCollection<SavedView> views,
        Guid userId,
        bool isNgoAdmin,
        CancellationToken ct)
    {
        var creatorIds = views.Select(x => x.CreatedBy).Distinct().ToList();
        var names = await context.Users
            .AsNoTracking()
            .Where(x => creatorIds.Contains(x.Id))
            .Select(x => new { x.Id, x.DisplayName })
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);

        return views
            .Select(view => SavedViewModel.FromEntity(view,
                names.GetValueOrDefault(view.CreatedBy, view.CreatedByName),
                CanEdit(view, userId, isNgoAdmin),
                CanChangeAccess(view, userId)))
            .ToList();
    }
}
