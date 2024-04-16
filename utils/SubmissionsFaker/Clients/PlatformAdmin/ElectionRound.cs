namespace SubmissionsFaker.Clients.PlatformAdmin;

public class ElectionRound
{
    public Guid CountryId { get; init; }
    public string Title { get; init; }
    public string EnglishTitle { get; init; }
    public DateOnly StartDate { get; init; }
}
