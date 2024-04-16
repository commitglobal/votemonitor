using System.Text.Json.Serialization;

namespace Vote.Monitor.Module.Notifications.Expo.Models;

public class PushReceiptRequest
{

    [JsonPropertyName("ids")]
    public List<string> PushTicketIds { get; set; }
}
