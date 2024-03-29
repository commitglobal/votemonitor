namespace Vote.Monitor.Core.Services.FileStorage.S3;

public class S3Options
{
    public const string SectionName = "S3";
    public string BucketName { get; set; }
    public int PresignedUrlValidityInSeconds{ get; set; }
}
