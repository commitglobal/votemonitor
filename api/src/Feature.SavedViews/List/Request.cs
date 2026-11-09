namespace Feature.SavedViews.List;

public class Request : SavedViewsRequest
{
    [QueryParam]
    public ReportRecordType RecordType { get; set; } = null!;
}
