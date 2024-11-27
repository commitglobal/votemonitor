using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Fakers;

public sealed class IncidentReportFaker : Faker<IncidentReportRequest>
{
    
    private readonly string[] _locationTypes = ["PollingStation", "OtherLocation"];

    public IncidentReportFaker(List<UpdateFormResponse> forms,List<PollingStationNode> pollingStations, List<LoginResponse> observers)
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.ObserverToken, f => f.PickRandom(observers).Token);
        
        Rules((f, x) =>
        {
            x.LocationType = f.PickRandom(_locationTypes);

            if (x.LocationType == "PollingStation")
            {
                x.PollingStationId = f.PickRandom(pollingStations).PollingStationId;
            }

            if (x.LocationType == "OtherLocation")
            {
                x.LocationDescription = f.Lorem.Sentence(10).OfLength(1024);
            }
        });
        
        Rules((f, x) =>
        {
            var form = f.PickRandom(forms);

            x.FormId = form.Id;
            x.Answers = f.PickRandom(form.Questions, f.Random.Int(0, form.Questions.Count))
                .Select(Answers.GetFakeAnswer).ToList();
        });
    }
}