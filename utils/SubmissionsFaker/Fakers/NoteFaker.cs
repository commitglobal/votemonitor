using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Fakers;

public sealed class NoteFaker : Faker<NoteRequest>
{
    public NoteFaker(List<UpdateFormResponse> forms, List<LocationNode> pollingStations, List<LoginResponse> observers)
    {
        RuleFor(x => x.PollingStationId, f => f.PickRandom(pollingStations).PollingStationId!);
        Rules((f, x) =>
        {
            var form = f.PickRandom(forms);

            x.FormId = form.Id;
            x.QuestionId = f.PickRandom(form.Questions).Id;
        });

        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Text, f => f.Lorem.Sentence(1000).Substring(0, 1000));
        RuleFor(x => x.ObserverToken, f => f.PickRandom(observers).Token);
    }
}