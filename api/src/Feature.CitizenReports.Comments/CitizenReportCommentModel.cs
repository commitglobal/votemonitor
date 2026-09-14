namespace Feature.CitizenReports.Comments;

public record CitizenReportCommentModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid CitizenReportId { get; init; }
    public required string Text { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string CreatedByName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public static CitizenReportCommentModel FromEntity(CitizenReportCommentAggregate comment)
        => new()
        {
            Id = comment.Id,
            ElectionRoundId = comment.ElectionRoundId,
            CitizenReportId = comment.CitizenReportId,
            Text = comment.Text,
            CreatedBy = comment.CreatedBy,
            CreatedByName = string.Empty,
            CreatedAt = comment.CreatedOn,
            LastModifiedAt = comment.LastModifiedOn
        };
}
