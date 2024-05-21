namespace Vote.Monitor.Module.Notifications.Expo;

public class ExpoOptions
{
    public const string SectionName = "Expo";
    public string Token { get; set; }
    public int BatchSize { get; set; } = 256;
    public string ChannelId { get; set; } = "default";
    public string Priority { get; set; } = "high";  // 'default' | 'normal' | 'high'
    public int TtlSeconds { get; set; } = 259200; // 3 days

}
