using Refit;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Clients.PlatformAdmin;

public interface IPlatformAdminApi
{
    [Post("/api/election-rounds")]
    Task<CreateResponse> CreateElectionRound([Body] ElectionRound electionRound, [Authorize] string token);

    [Post("/api/ngos")]
    Task<CreateResponse> CreateNgo([Body] Ngo ngo, [Authorize] string token);

    [Post("/api/ngos/{ngoId}/admins")]
    Task CreateNgoAdmin([Body] ApplicationUser ngoAdmin, [AliasAs("ngoId")] string ngoId, [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/polling-stations:import")]
    Task CreatePollingStations([AliasAs("electionRoundId")] string electionRoundId, [AliasAs("File")] StreamPart stream, [Authorize] string token);

    [Post("/api/observers")]
    Task<CreateResponse> CreateObserver([Body] ApplicationUser observer, [Authorize] string token);
    
    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos")]
    Task<CreateResponse> AssignNgoToElectionRound([AliasAs("electionRoundId")] string electionRoundId, [Body] AssignNgoRequest request, [Authorize] string token);
    
    [Post("/api/election-rounds/{electionRoundId}:enableCitizenReporting")]
    Task EnableCitizenReporting([AliasAs("electionRoundId")] string electionRoundId, [Body] EnableCitizenReportingRequest request, [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers")]
    Task<CreateResponse> AssignObserverToMonitoring([AliasAs("electionRoundId")] string electionRoundId, [AliasAs("monitoringNgoId")] string monitoringNgoId, [Body]AssignObserverRequest request, [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/polling-station-information-form")]
    Task CreatePSIForm([AliasAs("electionRoundId")] string electionRoundId, [Body] UpsertPSIFormRequest psiFormRequest, [Authorize] string token);

}