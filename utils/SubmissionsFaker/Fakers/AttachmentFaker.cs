using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class AttachmentFaker : Faker<AttachmentRequest>
{
    public AttachmentFaker(List<SubmissionRequest> submissions)
    {
        Rules((f, x) =>
        {
            var submission = f.PickRandom(submissions);

            x.Id = f.Random.Guid();
            x.FormId = submission.FormId;
            x.QuestionId = f.PickRandom(submission.Answers).QuestionId;
            x.PollingStationId = submission.PollingStationId;
            x.ObserverToken = submission.ObserverToken;
        });
    }
}