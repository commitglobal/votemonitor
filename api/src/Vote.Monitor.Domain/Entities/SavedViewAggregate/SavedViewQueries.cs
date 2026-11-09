namespace Vote.Monitor.Domain.Entities.SavedViewAggregate;

public static class SavedViewQueries
{
    /// <summary>
    /// Removes the private views of a deleted NGO member's account; their shared views stay, still naming them.
    /// </summary>
    public static Task DeletePrivateViewsAsync(this VoteMonitorContext context, Guid userId, CancellationToken ct) =>
        context.SavedViews
            .Where(x => x.CreatedBy == userId && x.Access == SavedViewAccess.Private)
            .ExecuteDeleteAsync(ct);
}
