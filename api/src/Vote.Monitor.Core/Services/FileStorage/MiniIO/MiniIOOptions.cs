namespace Vote.Monitor.Core.Services.FileStorage.MiniIO;

public class MiniIOOptions
{
    public const string SectionName = "MiniIO";
    public string BucketName { get; set; } = "user-uploads";
    public int PresignedUrlValidityInSeconds { get; set; }
}
