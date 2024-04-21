namespace Feature.Form.Submissions.ListByForm;

public record Response
{
    public List<AggregatedFormOverview> AggregatedForms { get; set; } = [];
}

public class AggregatedFormOverview
{
    public string FormId { get; set; }
    public string FormCode { get; set; }
    public string FormType { get; set; }
    public int NumberOfSubmissions { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfNotes { get; set; }
    public int NumberOfMediaFiles { get; set; }
}
