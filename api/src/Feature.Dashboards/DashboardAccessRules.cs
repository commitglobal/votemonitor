using System.Security.Claims;
using Feature.Dashboards.Models;
using Feature.Review;

namespace Feature.Dashboards;

/// <summary>Who can see and change the NGO's dashboards (docs/dashboards-backend.md in the web app).</summary>
public static class DashboardAccessRules
{
    /// <summary>
    /// The caller's monitoring NGO in the round when they are an admin or staff member of an active NGO that monitors
    /// it, also in archived rounds. Dashboards chart form submissions, so this is who can work on them.
    /// </summary>
    public static async Task<Guid?> ResolveMonitoringNgoIdAsync(this VoteMonitorContext context,
        IAuthorizationService authorizationService,
        ClaimsPrincipal user,
        Guid electionRoundId,
        Guid ngoId,
        CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(user, electionRoundId, ReportRecordType.FormSubmission))
        {
            return null;
        }

        return await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
    }

    /// <summary>
    /// The dashboards as the caller sees them, each with whether they may change it (NGO admins); the creator's current
    /// name, or the one kept with the dashboard when their account is gone.
    /// </summary>
    public static async Task<List<DashboardModel>> ToModelsAsync(this VoteMonitorContext context,
        IReadOnlyCollection<Dashboard> dashboards,
        bool isNgoAdmin,
        CancellationToken ct)
    {
        var creatorIds = dashboards.Select(x => x.CreatedBy).Distinct().ToList();
        var names = await context.Users
            .AsNoTracking()
            .Where(x => creatorIds.Contains(x.Id))
            .Select(x => new { x.Id, x.DisplayName })
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);

        return dashboards
            .Select(dashboard => DashboardModel.FromEntity(dashboard,
                names.GetValueOrDefault(dashboard.CreatedBy, dashboard.CreatedByName),
                isNgoAdmin))
            .ToList();
    }
}
