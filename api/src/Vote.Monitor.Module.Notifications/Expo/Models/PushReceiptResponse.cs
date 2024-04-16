using System.Text.Json.Serialization;

namespace Vote.Monitor.Module.Notifications.Expo.Models;

public class PushReceiptResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, PushTicketDeliveryStatus>? PushTicketReceipts { get; set; }

    [JsonPropertyName("errors")]
    public List<PushReceiptErrorInformation>? ErrorInformation { get; set; }
}

public class PushTicketDeliveryStatus
{
    [JsonPropertyName("status")]
    public string DeliveryStatus { get; set; }

    [JsonPropertyName("message")]
    public string DeliveryMessage { get; set; }

    [JsonPropertyName("details")]
    public object DeliveryDetails { get; set; }
}

public class PushReceiptErrorInformation
{
    [JsonPropertyName("code")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string ErrorMessage { get; set; }
}
