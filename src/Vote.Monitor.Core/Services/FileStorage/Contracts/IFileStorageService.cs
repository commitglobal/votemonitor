namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public interface IFileStorageService
{
    Task<UploadFileResult> UploadFileAsync(string uploadFolder, string fileName, Stream stream, CancellationToken ct = default);
    Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadFolder, string filename, CancellationToken ct = default);
}
