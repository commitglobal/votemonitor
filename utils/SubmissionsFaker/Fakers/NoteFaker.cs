using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class NoteFaker : Faker<NoteRequest>
{
    public NoteFaker(List<SubmissionRequest> submissions)
    {
        Rules((f, x) =>
        {
            var submission = f.PickRandom(submissions);

            x.Id = f.Random.Guid();
            x.PollingStationId = submission.PollingStationId;
            x.FormId = submission.FormId;
            x.QuestionId = f.PickRandom(submission.Answers).QuestionId;
            x.Text = f.Lorem.Sentence(100).OfLength(1000);
            x.ObserverToken = submission.ObserverToken;
        });
    }
}