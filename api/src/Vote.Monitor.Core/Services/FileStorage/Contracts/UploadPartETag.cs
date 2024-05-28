namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public class UploadPartETag
{
    public int PartNumber { get; set; }
    public string ETag { get; set; }
}
