using Spectre.Console;
using SubmissionsFaker.Clients;
using SubmissionsFaker.Clients.PlatformAdmin;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Seeders;

public class ObserversSeeder
{
    public static async Task<List<CreateResponse>> Seed(IPlatformAdminApi platformAdminApi,
        LoginResponse platformAdminToken,
        List<ApplicationUser> observers,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ProgressTask progressTask)
    {
        progressTask.StartTask();
        
        var observerIds = new List<CreateResponse>();

        foreach (var observersChunk in observers.Chunk(25))
        {
            var tasks = new List<Task<CreateResponse>>();
            foreach (var observer in observersChunk)
            {
                var createTask = platformAdminApi.CreateObserver(observer, platformAdminToken.Token);
                tasks.Add(createTask);
            }

            var observersChunkIds = await Task.WhenAll(tasks);
            progressTask.Increment(25);
            observerIds.AddRange(observersChunkIds);
        }

        foreach (var observersIdsChunk in observerIds.Chunk(25))
        {
            var tasks = new List<Task<CreateResponse>>();
            foreach (var observer in observersIdsChunk)
            {
                var assignTask = platformAdminApi.AssignObserverToMonitoring(electionRoundId, monitoringNgoId, new AssignObserverRequest(observer.Id), platformAdminToken.Token);
                tasks.Add(assignTask);
            }

            await Task.WhenAll(tasks);
            progressTask.Increment(25);
        }

        return observerIds;
    }
}