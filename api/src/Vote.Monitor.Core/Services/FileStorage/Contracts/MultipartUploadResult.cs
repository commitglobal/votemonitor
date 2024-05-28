namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public class MultipartUploadResult
{
    public string UploadId { get; set; }
    public Dictionary<int, string> PresignedUrls { get; set; } = new();
}

