using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.LocalDisk;

internal class LocalDiskFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalDiskFileStorageService> _logger;
    private readonly LocalDiskOptions _options;

    public LocalDiskFileStorageService(IOptions<LocalDiskOptions> options, ILogger<LocalDiskFileStorageService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<UploadFileResult> UploadFileAsync(string uploadFolder, string filename, Stream stream, CancellationToken ct = default)
    {
        try
        {
            if (!Directory.Exists(_options.Path))
            {
                Directory.CreateDirectory(_options.Path);
            }

            var fullPath = Path.Combine(uploadFolder, filename);

            await using (var fileStream = File.Create(fullPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream, ct);
            }

            var result = await GetPresignedUrlAsync(uploadFolder, filename, ct);
            if (result is GetPresignedUrlResult.Ok file)
            {
                return new UploadFileResult.Ok(file.Url, file.Filename);
            }

            // Get presigned url failed but upload succeed
            return new UploadFileResult.Ok(string.Empty, string.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not upload file {uploadFolder} {filename}", uploadFolder, filename);
            return new UploadFileResult.Failed();
        }
    }

    public async Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadFolder, string filename, CancellationToken ct = default)
    {
        try
        {
            await Task.CompletedTask;

            var fullPath = Path.Combine(uploadFolder, filename);

            if (!File.Exists(fullPath))
            {
                return new GetPresignedUrlResult.NotFound();
            }

            return new GetPresignedUrlResult.Ok(fullPath, filename);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not get presigned url for file {uploadFolder} {filename}", uploadFolder, filename);
        }

        return new GetPresignedUrlResult.Failed();
    }
}
