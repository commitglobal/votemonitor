using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Feature.Review.Services;

/// <summary>Queries shared by the people's and the automation rules' review changes.</summary>
internal static class ReviewDbExtensions
{
    /// <summary>The active NGO admins and staff among the given users, with their display names.</summary>
    public static async Task<Dictionary<Guid, string>> GetAssignableUsersAsync(this VoteMonitorContext context,
        Guid ngoId,
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        if (userIds.Count == 0)
        {
            return [];
        }

        var admins = await context.NgoAdmins
            .Where(x => x.NgoId == ngoId
                        && userIds.Contains(x.ApplicationUserId)
                        && x.ApplicationUser.Status == UserStatus.Active)
            .Select(x => new { x.ApplicationUserId, x.ApplicationUser.DisplayName })
            .ToListAsync(ct);
        var staff = await context.Set<NgoStaff>()
            .Where(x => x.NgoId == ngoId
                        && userIds.Contains(x.ApplicationUserId)
                        && x.ApplicationUser.Status == UserStatus.Active)
            .Select(x => new { x.ApplicationUserId, x.ApplicationUser.DisplayName })
            .ToListAsync(ct);

        return admins.Concat(staff)
            .DistinctBy(x => x.ApplicationUserId)
            .ToDictionary(x => x.ApplicationUserId, x => x.DisplayName);
    }

    /// <summary>Adds an inbox item for every active recipient except the user who made the change.</summary>
    public static async Task NotifyAsync(this VoteMonitorContext context,
        ReviewActivity activity,
        IEnumerable<Guid> recipients,
        UserNotificationReason reason,
        CancellationToken ct)
    {
        var candidates = recipients.Where(x => x != activity.ActorId).Distinct().ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var active = await context.Users
            .Where(x => candidates.Contains(x.Id) && x.Status == UserStatus.Active)
            .Select(x => x.Id)
            .ToListAsync(ct);

        foreach (var userId in active)
        {
            context.UserNotifications.Add(UserNotification.Create(userId, activity, reason));
        }
    }
}
