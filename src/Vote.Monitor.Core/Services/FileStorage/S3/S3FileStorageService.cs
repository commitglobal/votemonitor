using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _client;
    private readonly S3Options _options;

    public S3FileStorageService(IAmazonS3 client, IOptions<S3Options> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<UploadFileResult> UploadFileAsync(string uploadPath, string fileName, Stream stream, CancellationToken ct = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = GetFileKey(uploadPath, fileName),
            InputStream = stream
        };

        try
        {
            var response = await _client.PutObjectAsync(request, ct);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var presignedUrlResult = await GetPresignedUrlAsync(uploadPath, fileName, ct);
                
                //upload succeeded but presigned url failed 
                if (presignedUrlResult is GetPresignedUrlResult.Failed)
                {
                    return new UploadFileResult.Ok(string.Empty, fileName, 0);
                }

                var okPresignedResult = presignedUrlResult as GetPresignedUrlResult.Ok;
                return new UploadFileResult.Ok(okPresignedResult!.Url, fileName, _options.PresignedUrlValidityInSeconds);
            }
        }
        catch (AmazonS3Exception ex)
        {
            return new UploadFileResult.Failed($"Failed to upload file to S3: {ex.Message}");
        }

        return new UploadFileResult.Failed("Failed to upload file to S3");
    }

    public async Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadPath, string filename, CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = GetFileKey(uploadPath, filename),
            Expires = DateTime.UtcNow.AddSeconds(_options.PresignedUrlValidityInSeconds),
        };

        try
        {
            var urlString = await _client.GetPreSignedURLAsync(request);

            return new GetPresignedUrlResult.Ok(urlString, filename);
        }
        catch (AmazonS3Exception ex)
        {
            return new GetPresignedUrlResult.Failed($"Failed to generate presigned Url for S3: {ex.Message}");
        }
    }

    private static string GetFileKey(string uploadPath, string fileName)
    {
        return $"{uploadPath}/{fileName}";
    }
}
