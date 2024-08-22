using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Vote.Monitor.Core.Services.FileStorage.S3;

internal class S3FileStorageService(IAmazonS3 client,
    ILogger<S3FileStorageService> logger,
    IOptions<S3Options> options) : IFileStorageService
{
    private readonly S3Options _options = options.Value;

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
            var response = await client.PutObjectAsync(request, ct);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var presignedUrlResult = await GetPresignedUrlAsync(uploadPath, fileName);

                //upload succeeded but presigned url failed 
                if (presignedUrlResult is GetPresignedUrlResult.Failed)
                {
                    return new UploadFileResult.Ok(string.Empty, fileName, 0);
                }

                var okPresignedResult = presignedUrlResult as GetPresignedUrlResult.Ok;
                return new UploadFileResult.Ok(okPresignedResult!.Url, fileName, _options.PresignedUrlValidityInSeconds);
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            logger.LogError(ex, "Failed to upload file {uploadPath} {fileName}", uploadPath, fileName);
        }

        return new UploadFileResult.Failed("Failed to upload file to S3");
    }

    public async Task<GetPresignedUrlResult> GetPresignedUrlAsync(string uploadPath, string fileName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = GetFileKey(uploadPath, fileName),
            Expires = DateTime.UtcNow.AddSeconds(_options.PresignedUrlValidityInSeconds),
        };

        try
        {
            var urlString = await client.GetPreSignedURLAsync(request);

            return new GetPresignedUrlResult.Ok(urlString, fileName, _options.PresignedUrlValidityInSeconds);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            logger.LogError(ex, "Failed to generate presigned Url in S3 for {uploadPath} {fileName}", uploadPath, fileName);
        }

        return new GetPresignedUrlResult.Failed("Failed to generate presigned Url");
    }

    public async Task<MultipartUploadResult> CreateMultipartUploadAsync(string uploadPath, string fileName, string contentType, int numberOfUploadParts, CancellationToken ct = default)
    {
        try
        {
            var fileKey = GetFileKey(uploadPath, fileName);
            var request = new InitiateMultipartUploadRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                ContentType = contentType,
            };

            var response = await client.InitiateMultipartUploadAsync(request, ct);
            var presignedUrls = new Dictionary<int, string>();

            for (int partNumber = 1; partNumber <= numberOfUploadParts; partNumber++)
            {
                var partPresignedUrl = client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    UploadId = response.UploadId,
                    BucketName = _options.BucketName,
                    PartNumber = partNumber,
                    Key = fileKey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddHours(24),
                });

                presignedUrls.Add(partNumber, partPresignedUrl);
            }

            return new MultipartUploadResult { UploadId = response.UploadId, PresignedUrls = presignedUrls };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create multipart upload in S3 for {uploadPath} {fileName}", uploadPath, fileName);
            throw;
        }
    }

    public async Task CompleteUploadAsync(string uploadId, string uploadPath, string fileName, Dictionary<int, string> eTags, CancellationToken ct = default)
    {
        try
        {
            var fileKey = GetFileKey(uploadPath, fileName);
            var request = new CompleteMultipartUploadRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                PartETags = eTags.Select(x => new PartETag(x.Key, x.Value)).ToList(),
                UploadId = uploadId
            };

            _ = await client.CompleteMultipartUploadAsync(request, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to complete multipart upload in S3 for {uploadId} {uploadPath} {fileName}", uploadId, uploadPath, fileName);
            throw;
        }
    }

    public async Task AbortUploadAsync(string uploadId, string uploadPath, string fileName, CancellationToken ct)
    {
        try
        {
            var fileKey = GetFileKey(uploadPath, fileName);
            var request = new AbortMultipartUploadRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                UploadId = uploadId
            };

            _ = await client.AbortMultipartUploadAsync(request, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to abort multipart upload in S3 for {uploadId} {uploadPath} {fileName}", uploadId, uploadPath, fileName);
            throw;
        }
    }

    private static string GetFileKey(string uploadPath, string fileName)
    {
        return $"{uploadPath}/{fileName}";
    }
}
