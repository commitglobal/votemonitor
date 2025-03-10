namespace Vote.Monitor.Api.Feature.Observer.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public UserStatus? Status { get; set; }
    [QueryParam] public bool? IsEmailVerified { get; set; }
}
