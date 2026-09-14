namespace Feature.QuickReports.Comments;

public record QuickReportCommentModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid QuickReportId { get; init; }
    public required string Text { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string CreatedByName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public static QuickReportCommentModel FromEntity(QuickReportCommentAggregate comment)
        => new()
        {
            Id = comment.Id,
            ElectionRoundId = comment.ElectionRoundId,
            QuickReportId = comment.QuickReportId,
            Text = comment.Text,
            CreatedBy = comment.CreatedBy,
            CreatedByName = string.Empty,
            CreatedAt = comment.CreatedOn,
            LastModifiedAt = comment.LastModifiedOn
        };
}
