using System.Text.Json.Serialization;

namespace Vote.Monitor.Module.Notifications.Expo.Models;

public class PushTicketRequest
{
    [JsonPropertyName("to")]
    public List<string> PushTo { get; set; }

    [JsonPropertyName("data")]
    public object PushData { get; set; }

    [JsonPropertyName("title")]
    public string PushTitle { get; set; }

    [JsonPropertyName("body")]
    public string PushBody { get; set; }

    [JsonPropertyName("ttl")]
    public int? PushTTL { get; set; }

    [JsonPropertyName("expiration")]
    public int? PushExpiration { get; set; }

    [JsonPropertyName("priority")]  //'default' | 'normal' | 'high'
    public string PushPriority { get; set; }

    [JsonPropertyName("subtitle")]
    public string PushSubTitle { get; set; }

    [JsonPropertyName("sound")] //'default' | null	
    public string PushSound { get; set; }

    [JsonPropertyName("badge")]
    public int? PushBadgeCount { get; set; }

    [JsonPropertyName("channelId")]
    public string PushChannelId { get; set; }

    [JsonPropertyName("categoryId")]
    public string PushCategoryId { get; set; }
}
