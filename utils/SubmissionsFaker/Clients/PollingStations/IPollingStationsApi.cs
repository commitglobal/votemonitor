using Refit;

namespace SubmissionsFaker.Clients.PollingStations;

public interface IPollingStationsApi
{
    [Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAll")]
    Task<AllPollingStations> GetAllPollingStations([AliasAs("electionRoundId")] string electionRoundId, [Authorize] string token);
}