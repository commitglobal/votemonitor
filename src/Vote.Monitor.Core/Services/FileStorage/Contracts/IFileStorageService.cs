namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public interface IFileStorageService
{
    Task<UploadFileResult> UploadFileAsync(string uploadPath, string fileName, Stream stream, CancellationToken ct = default);
    Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadPath, string fileName, CancellationToken ct = default);
}
