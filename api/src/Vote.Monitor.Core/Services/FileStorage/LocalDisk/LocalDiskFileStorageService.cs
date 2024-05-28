using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.LocalDisk;

internal class LocalDiskFileStorageService(
    IOptions<LocalDiskOptions> options,
    ILogger<LocalDiskFileStorageService> logger)
    : IFileStorageService
{
    private const int UrlValidityInSeconds = 99999;
    private readonly LocalDiskOptions _options = options.Value;

    public async Task<UploadFileResult> UploadFileAsync(string uploadPath, string filename, Stream stream, CancellationToken ct = default)
    {
        try
        {
            var uploadDirectory = Path.Combine(_options.Path, uploadPath);
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fullPath = Path.Combine(_options.Path, uploadPath, filename);

            await using (var fileStream = File.Create(fullPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream, ct);
            }

            var result = await GetPresignedUrlAsync(uploadPath, filename);
            if (result is GetPresignedUrlResult.Ok file)
            {
                return new UploadFileResult.Ok(file.Url, file.Filename, UrlValidityInSeconds);
            }

            // Get presigned url failed but upload succeed
            return new UploadFileResult.Ok(string.Empty, string.Empty, 0);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not upload file {uploadPath} {filename}", uploadPath, filename);
            return new UploadFileResult.Failed($"Failed to upload file to LocalDisk: {e.Message}");
        }
    }

    public async Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadPath, string fileName)
    {
        try
        {
            await Task.CompletedTask;

            var fullPath = Path.Combine(_options.Path, uploadPath, fileName);

            if (!File.Exists(fullPath))
            {
                return new GetPresignedUrlResult.NotFound();
            }

            return new GetPresignedUrlResult.Ok(fullPath, fileName, UrlValidityInSeconds);

        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not get presigned url for file {uploadPath} {filename}", uploadPath, fileName);
            return new GetPresignedUrlResult.Failed($"Failed to generate presigned url for LocalDisk: {e.Message}");
        }
    }

    public Task<MultipartUploadResult> CreateMultipartUploadAsync(string uploadPath, string fileName, string contentType, int numberOfUploadParts,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task CompleteUploadAsync(string uploadId, string uploadPath, string fileName, Dictionary<int, string> eTags,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task AbortUploadAsync(string uploadId, string uploadPath, string fileName, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
