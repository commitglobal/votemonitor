namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public interface IFileStorageService
{
    Task<UploadFileResult> UploadFileAsync(string uploadPath, string fileName, Stream stream, CancellationToken ct = default);
    Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadPath, string fileName);
    Task<MultipartUploadResult> CreateMultipartUploadAsync(string uploadPath, string fileName, string contentType,
        int numberOfUploadParts, CancellationToken ct = default);
    Task CompleteUploadAsync(string uploadId, string uploadPath, string fileName, Dictionary<int, string> eTags, CancellationToken ct = default);
    Task AbortUploadAsync(string uploadId, string uploadPath, string fileName, CancellationToken ct);
}
