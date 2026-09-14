namespace Feature.IncidentReports.Comments;

public record IncidentReportCommentModel
{
    public required Guid Id { get; init; }
    public required Guid ElectionRoundId { get; init; }
    public required Guid IncidentReportId { get; init; }
    public required string Text { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string CreatedByName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public static IncidentReportCommentModel FromEntity(IncidentReportCommentAggregate comment)
        => new()
        {
            Id = comment.Id,
            ElectionRoundId = comment.ElectionRoundId,
            IncidentReportId = comment.IncidentReportId,
            Text = comment.Text,
            CreatedBy = comment.CreatedBy,
            CreatedByName = string.Empty,
            CreatedAt = comment.CreatedOn,
            LastModifiedAt = comment.LastModifiedOn
        };
}
