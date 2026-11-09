using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.Export;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    // The list's filters, so the file holds the observers the list shows (none: all of them)
    [QueryParam]
    public string[]? Tags { get; set; } = [];

    [QueryParam]
    public string? SearchText { get; set; }

    /// <summary>Only observers with any of these statuses (repeat the key for several).</summary>
    [QueryParam]
    public MonitoringObserverStatus[]? Status { get; set; }
}
