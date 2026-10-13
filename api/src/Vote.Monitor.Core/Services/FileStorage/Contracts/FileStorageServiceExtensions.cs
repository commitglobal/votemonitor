using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public static class FileStorageServiceExtensions
{
    /// <summary>
    /// Returns the attachments with a presigned download url; attachments that could not be presigned are returned unchanged.
    /// </summary>
    public static async Task<T[]> PresignAsync<T>(
        this IFileStorageService fileStorageService, IEnumerable<T> attachments) where T : AttachmentModel
    {
        return await Task.WhenAll(attachments.Select(async attachment =>
        {
            var result =
                await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
            return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                ? (T)(attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds })
                : attachment;
        }));
    }
}
