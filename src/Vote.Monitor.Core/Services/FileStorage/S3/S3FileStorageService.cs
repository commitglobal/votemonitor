using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal class S3FileStorageService : IFileStorageService
{
    public S3FileStorageService()
    {
    }

    public Task<UploadFileResult> UploadFileAsync(string uploadFolder, string fileName, Stream stream, CancellationToken ct = default)
    {
        throw new NotImplementedException("Implementation is out of the scope");
    }

    public Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadFolder, string filename, CancellationToken ct = default)
    {
        throw new NotImplementedException("Implementation is out of the scope");
    }
}
