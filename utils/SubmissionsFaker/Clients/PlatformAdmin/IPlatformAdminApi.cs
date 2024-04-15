﻿using Refit;

namespace SubmissionsFaker.Clients.PlatformAdmin;

public interface IPlatformAdminApi
{
    [Post("/api/election-rounds")]
    Task<CreateResponse> CreateElectionRound([Body] ElectionRound electionRound, [Authorize] string token);

    [Post("/api/ngos")]
    Task<CreateResponse> CreateNgo([Body] Ngo ngo, [Authorize] string token);

    [Post("/api/ngos/{ngoId}/admins")]
    Task CreateNgoAdmin([Body] ApplicationUser ngoAdmin, [AliasAs("ngoId")] Guid ngoId, [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/polling-stations:import")]
    Task CreatePollingStations([AliasAs("electionRoundId")] Guid electionRoundId, [AliasAs("File")] StreamPart stream, [Authorize] string token);

    [Post("/api/observers")]
    Task<CreateResponse> CreateObserver([Body] ApplicationUser observer, [Authorize] string token);
    
    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos")]
    Task<CreateResponse> AssignNgoToElectionRound([AliasAs("electionRoundId")] Guid electionRoundId, [Body] AssignNgoRequest request, [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers")]
    Task<CreateResponse> AssignObserverToMonitoring([AliasAs("electionRoundId")] Guid electionRoundId, [AliasAs("monitoringNgoId")] Guid monitoringNgoId, [Body]AssignObserverRequest request, [Authorize] string token);

}