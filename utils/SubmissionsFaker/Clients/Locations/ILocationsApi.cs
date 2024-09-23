using Refit;

namespace SubmissionsFaker.Clients.Locations;

public interface ILocationsApi
{
    [Get("/api/election-rounds/{electionRoundId}/locations:fetchAll")]
    Task<AllLocations> GetAllLocations([AliasAs("electionRoundId")] string electionRoundId, [Authorize] string token);
}