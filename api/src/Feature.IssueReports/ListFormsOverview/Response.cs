namespace Feature.IssueReports.ListFormsOverview;

public record Response
{
    public List<AggregatedFormOverview> AggregatedForms { get; set; } = [];
}

public class AggregatedFormOverview
{
    public Guid FormId { get; set; }
    public string FormCode { get; set; }
    public int NumberOfSubmissions { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfNotes { get; set; }
    public int NumberOfMediaFiles { get; set; }
    public TranslatedString FormName { get; set; }
    public string FormDefaultLanguage { get; set; }
}