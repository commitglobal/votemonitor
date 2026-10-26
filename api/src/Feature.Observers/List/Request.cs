namespace Feature.Observers.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    /// <summary>Only observers with any of these statuses (repeat the key for several).</summary>
    [QueryParam] public UserStatus[]? Status { get; set; }
    [QueryParam] public bool? IsEmailVerified { get; set; }
}
