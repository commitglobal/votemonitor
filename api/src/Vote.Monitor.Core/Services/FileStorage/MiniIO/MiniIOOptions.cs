namespace Vote.Monitor.Core.Services.FileStorage.MiniIO;

public class MiniIOOptions
{
    public const string SectionName = "MiniIO";
    public string BucketName { get; set; } = "user-uploads";
    public string Region { get; set; }
    public string EndpointUrl { get; set; }
    public int PresignedUrlValidityInSeconds { get; set; }
}
