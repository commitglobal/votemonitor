using Vote.Monitor.Core.Models;

namespace Feature.NgoStaff.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? SearchText { get; set; }

    /// <summary>Only staff with any of these statuses (repeat the key for several).</summary>
    [QueryParam]
    public UserStatus[]? Status { get; set; }
}
