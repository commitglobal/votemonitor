﻿namespace Feature.CitizenReports.Attachments;

public record CitizenReportsAttachmentModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; set; }
    public required Guid CitizenReportId { get; set; }
    public required Guid FormId { get; init; }
    public required Guid QuestionId { get; init; }
    public required string FileName { get; init; } = string.Empty;
    public required string MimeType { get; init; } = string.Empty;
    public required string PresignedUrl { get; init; } = string.Empty;
    public required int UrlValidityInSeconds { get; init; }
}