namespace Feature.CitizenReports.Comments.List;

public record Response
{
    public required List<CitizenReportCommentModel> Comments { get; init; }
}
