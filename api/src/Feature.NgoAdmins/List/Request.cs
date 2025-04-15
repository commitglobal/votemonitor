using Vote.Monitor.Core.Models;

namespace Feature.NgoAdmins.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus? Status { get; set; }
}
