namespace Vote.Monitor.Module.Notifications.Expo;

public class ExpoOptions
{
    public const string SectionName = "Expo";
    public string Token { get; set; }
    public int BatchSize { get; set; } = 256;
}
