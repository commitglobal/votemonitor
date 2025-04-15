namespace Feature.ElectionRounds.Create;

public class Request
{
    public Guid CountryId { get; set; }
    public string Title { get;  set; }
    public string EnglishTitle { get;  set; }
    public DateOnly StartDate { get;  set; }
}
