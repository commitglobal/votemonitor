namespace Vote.Monitor.Module.Notifications.Firebase;

public class FirebaseOptions
{
    public const string SectionName = "Firebase";
    public string Token { get; set; }
    public int BatchSize { get; set; } = 256;
}
