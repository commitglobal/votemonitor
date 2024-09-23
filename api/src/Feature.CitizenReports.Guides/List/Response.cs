namespace Feature.CitizenReports.Guides.List;

public record Response
{
    public required List<CitizenReportsGuideModel> Guides { get; set; }
}
