namespace Feature.SavedViews.Create;

public class Request : SaveViewRequest
{
    public ReportRecordType RecordType { get; set; } = null!;
}
