using Refit;
using SubmissionsFaker.Clients.Locations;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.PlatformAdmin.Models;
using SubmissionsFaker.Clients.PollingStations;

namespace SubmissionsFaker.Clients.PlatformAdmin;

public interface IPlatformAdminApi
{
    [Post("/api/election-rounds")]
    Task<CreateResponse> CreateElectionRound([Body] ElectionRound electionRound);

    [Post("/api/ngos")]
    Task<CreateResponse> CreateNgo([Body] Ngo ngo);

    [Post("/api/ngos/{ngoId}/admins")]
    Task CreateNgoAdmin([Body] ApplicationUser ngoAdmin, [AliasAs("ngoId")] Guid ngoId);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/polling-stations:import")]
    Task ImportPollingStations([AliasAs("electionRoundId")] Guid electionRoundId, [AliasAs("File")] StreamPart stream);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/locations:import")]
    Task ImportLocations([AliasAs("electionRoundId")] Guid electionRoundId, [AliasAs("File")] StreamPart stream);

    [Post("/api/observers")]
    Task<CreateResponse> CreateObserver([Body] ApplicationUser observer);

    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos")]
    Task<CreateResponse> AssignNgoToElectionRound([AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] AssignNgoRequest request);

    [Post("/api/election-rounds/{electionRoundId}:enableCitizenReporting")]
    Task EnableCitizenReporting([AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] EnableCitizenReportingRequest request);

    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers")]
    Task<CreateResponse> AssignObserverToMonitoring([AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("monitoringNgoId")] Guid monitoringNgoId, [Body] AssignObserverRequest request);

    [Post("/api/election-rounds/{electionRoundId}/polling-station-information-form")]
    Task CreatePSIForm([AliasAs("electionRoundId")] Guid electionRoundId, [Body] UpsertPSIFormRequest psiFormRequest);

    [Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAll")]
    Task<AllPollingStations> GetAllPollingStations([AliasAs("electionRoundId")] Guid electionRoundId);

    [Get("/api/election-rounds/{electionRoundId}/locations:fetchAll")]
    Task<AllLocations> GetAllLocations([AliasAs("electionRoundId")] Guid electionRoundId);
}