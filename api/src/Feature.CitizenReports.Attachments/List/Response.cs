namespace Feature.CitizenReports.Attachments.List;

public record Response
{
    public CitizenReportsAttachmentModel[] Attachments { get; init; } = [];
}