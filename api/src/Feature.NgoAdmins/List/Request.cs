using Vote.Monitor.Core.Models;

namespace Feature.NgoAdmins.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? SearchText { get; set; }

    /// <summary>Only admins with any of these statuses (repeat the key for several).</summary>
    [QueryParam]
    public UserStatus[]? Status { get; set; }
}
