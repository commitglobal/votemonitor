using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public static class FileStorageServiceExtensions
{
    /// <summary>
    /// Returns the attachments with a presigned download url; attachments that could not be presigned are returned unchanged.
    /// </summary>
    public static async Task<AggregatedSubmissionsAttachmentModel[]> PresignAsync(
        this IFileStorageService fileStorageService, IEnumerable<AggregatedSubmissionsAttachmentModel> attachments)
    {
        return await Task.WhenAll(attachments.Select(async attachment =>
        {
            var result =
                await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
            return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                : attachment;
        }));
    }
}
