using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class PISSubmissionFaker : Faker<PSISubmissionRequest>
{
    public PISSubmissionFaker(UpsertPSIFormRequest psiForm, Guid pollingStationId, string observerToken)
    {
        RuleFor(x => x.PollingStationId, pollingStationId);
        RuleFor(x => x.ObserverToken, observerToken);
        Rules((f, x) =>
        {
            var baseDate = f.Date.Recent(0, DateTime.UtcNow);
            x.ArrivalTime = baseDate;
            x.DepartureTime = baseDate.AddHours(f.Random.Double(0.5d));

            x.Answers = f
                .PickRandom(psiForm.Questions, f.Random.Int(0, psiForm.Questions.Count))
                .Select(Answers.GetFakeAnswer).ToList();
        });
    }
}