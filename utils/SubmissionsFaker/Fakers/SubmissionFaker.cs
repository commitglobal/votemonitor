using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Fakers;

public sealed class SubmissionFaker : Faker<SubmissionRequest>
{
    public SubmissionFaker(List<UpdateFormResponse> forms, List<LocationNode> pollingStations, List<LoginResponse> observers)
    {
        RuleFor(x => x.PollingStationId, f => f.PickRandom(pollingStations).PollingStationId!);
        RuleFor(x => x.ObserverToken, f => f.PickRandom(observers).Token);

        Rules((f, x) =>
        {
            var form = f.PickRandom(forms);

            x.FormId = form.Id;
            x.Answers = f.PickRandom(form.Questions, f.Random.Int(0, form.Questions.Count)).Select(Answers.GetFakeAnswer).ToList();
        });
    }    
}