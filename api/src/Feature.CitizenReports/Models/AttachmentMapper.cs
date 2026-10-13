using Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;

namespace Feature.CitizenReports.Models;

public static class AttachmentMapper
{
    public static SubmissionAttachmentModel ToModel(CitizenReportAttachment attachment) => new()
    {
        SubmissionId = attachment.CitizenReportId,
        QuestionId = attachment.QuestionId,
        FileName = attachment.FileName,
        MimeType = attachment.MimeType,
        FilePath = attachment.FilePath,
        UploadedFileName = attachment.UploadedFileName,
        TimeSubmitted = attachment.CreatedOn
    };
}
