using System.Text.Json.Serialization;

namespace Vote.Monitor.Module.Notifications.Expo.Models;


public class PushTicketResponse
{
    [JsonPropertyName("data")]
    public List<PushTicketStatus>? PushTicketStatuses { get; set; }

    [JsonPropertyName("errors")]
    public List<PushTicketErrors>? PushTicketErrors { get; set; }

}


public class PushTicketStatus
{

    [JsonPropertyName("status")] //"error" | "ok",
    public string TicketStatus { get; set; }

    [JsonPropertyName("id")]
    public string TicketId { get; set; }

    [JsonPropertyName("message")]
    public string TicketMessage { get; set; }

    [JsonPropertyName("details")]
    public object TicketDetails { get; set; }
}


public class PushTicketErrors
{
    [JsonPropertyName("code")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string ErrorMessage { get; set; }
}
