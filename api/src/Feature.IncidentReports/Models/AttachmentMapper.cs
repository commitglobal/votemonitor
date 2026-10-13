using Vote.Monitor.Domain.Entities.IncidentReportAttachmentAggregate;

namespace Feature.IncidentReports.Models;

public static class AttachmentMapper
{
    public static SubmissionAttachmentModel ToModel(IncidentReportAttachment attachment) => new()
    {
        SubmissionId = attachment.IncidentReportId,
        QuestionId = attachment.QuestionId,
        FileName = attachment.FileName,
        MimeType = attachment.MimeType,
        FilePath = attachment.FilePath,
        UploadedFileName = attachment.UploadedFileName,
        TimeSubmitted = attachment.CreatedOn
    };
}
