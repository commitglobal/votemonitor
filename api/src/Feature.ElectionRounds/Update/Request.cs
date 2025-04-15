namespace Feature.ElectionRounds.Update;

public class Request
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string EnglishTitle { get; set; }
    public DateOnly StartDate { get; set; }
    public Guid CountryId { get; set; }
}
