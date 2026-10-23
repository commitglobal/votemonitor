namespace Feature.Review.Models;

public record InboxItemModel
{
    public required Guid Id { get; init; }
    public required UserNotificationReason Reason { get; init; }
    public required DateTime CreatedOn { get; init; }
    public DateTime? ReadAt { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required string ElectionRoundTitle { get; init; }
    public required ReportRecordType RecordType { get; init; }
    public required Guid RecordId { get; init; }
    public required ReviewActivityModel Activity { get; init; }
}
