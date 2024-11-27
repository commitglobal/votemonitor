using Spectre.Console;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.PlatformAdmin;
using SubmissionsFaker.Clients.PlatformAdmin.Models;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Seeders;

public class ObserversSeeder
{
    public static async Task<List<CreateResponse>> Seed(IPlatformAdminApi platformAdminApi,
        List<ApplicationUser> observers,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ProgressTask progressTask)
    {
        progressTask.StartTask();

        var observerIds = new List<CreateResponse>();

        foreach (var observersChunk in observers.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = new List<Task<CreateResponse>>();
            foreach (var observer in observersChunk)
            {
                var createTask = platformAdminApi.CreateObserver(observer);
                tasks.Add(createTask);
            }

            var observersChunkIds = await Task.WhenAll(tasks);
            progressTask.Increment(25);
            observerIds.AddRange(observersChunkIds);
        }

        foreach (var observersIdsChunk in observerIds.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = new List<Task<CreateResponse>>();
            foreach (var observer in observersIdsChunk)
            {
                var assignTask = platformAdminApi.AssignObserverToMonitoring(electionRoundId, monitoringNgoId, new AssignObserverRequest(observer.Id));
                tasks.Add(assignTask);
            }

            await Task.WhenAll(tasks);
            progressTask.Increment(25);
        }

        return observerIds;
    }
}