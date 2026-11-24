namespace Vote.Monitor.Domain;

/// <summary>Looks up who saved a version of a record (concurrent edits contract).</summary>
public interface IUserDisplayNameProvider
{
    /// <summary>The user's display name; null when unknown (no user, account gone).</summary>
    Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken ct);
}

public class UserDisplayNameProvider(VoteMonitorContext context) : IUserDisplayNameProvider
{
    public Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken ct) =>
        context.GetUserDisplayNameAsync(userId, ct);
}

/// <summary>Helpers for the records clients save with a version (concurrent edits contract).</summary>
public static class RecordVersions
{
    /// <summary>The display name of the user who saved a version; null when unknown (no user, account gone).</summary>
    public static async Task<string?> GetUserDisplayNameAsync(this VoteMonitorContext context, Guid userId,
        CancellationToken ct)
    {
        if (userId == Guid.Empty)
        {
            return null;
        }

        return await context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Whether a save sent with <paramref name="sentVersion"/> was made from an older version than
    /// <paramref name="currentVersion"/>. A save without a version is never stale (older clients, the mobile app).
    /// </summary>
    public static bool IsStale(uint? sentVersion, uint currentVersion) =>
        sentVersion is { } version && version != currentVersion;
}
