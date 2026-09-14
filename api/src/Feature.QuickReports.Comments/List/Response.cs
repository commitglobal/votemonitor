namespace Feature.QuickReports.Comments.List;

public record Response
{
    public required List<QuickReportCommentModel> Comments { get; init; }
}
