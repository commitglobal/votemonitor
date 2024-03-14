namespace Vote.Monitor.Core.Services.PushNotification.Firebase;

public class FirebaseOptions
{
    public const string SectionName = "Firebase";
    public string Token { get; set; }
    public int BatchSize { get; set; } = 256;
}
