using Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;

namespace Feature.CitizenReports.Models;

public record AttachmentModel
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionId { get; init; }
    public string FilePath { get; init; }
    public string UploadedFileName { get; init; }
    public string FileName { get; init; }
    public string MimeType { get; init; }
    public string PresignedUrl { get; init; }
    public int UrlValidityInSeconds { get; init; }

    public DateTime TimeSubmitted { get; init; }

    public static AttachmentModel FromEntity(CitizenReportAttachment attachment)
    {
        return attachment == null
            ? null
            : new AttachmentModel
            {
                SubmissionId = attachment.CitizenReportId,
                QuestionId = attachment.QuestionId,
                FileName = attachment.FileName,
                MimeType = attachment.MimeType,
                FilePath = attachment.FilePath,
                UploadedFileName = attachment.UploadedFileName,
                PresignedUrl = string.Empty,
                UrlValidityInSeconds = int.MinValue,
                TimeSubmitted = attachment.CreatedOn
            };
    }
}