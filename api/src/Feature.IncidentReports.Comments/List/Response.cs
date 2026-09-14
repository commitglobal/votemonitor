namespace Feature.IncidentReports.Comments.List;

public record Response
{
    public required List<IncidentReportCommentModel> Comments { get; init; }
}
