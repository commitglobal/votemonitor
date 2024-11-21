using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Fakers;

public sealed class QuickReportRequestFaker : Faker<QuickReportRequest>
{
    public QuickReportRequestFaker(List<PollingStationNode> pollingStations, List<LoginResponse> observers)
    {
        RuleFor(x => x.PollingStationId, f => f.PickRandom(pollingStations).PollingStationId!);
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Lorem.Sentence(100).OfLength(1000));
        RuleFor(x => x.Description, f => f.Lorem.Sentence(1000).OfLength(10000));
        RuleFor(x => x.ObserverToken, f => f.PickRandom(observers).Token);
    }
    
    public QuickReportRequestFaker(Guid pollingStationId)
    {
        RuleFor(x => x.PollingStationId, pollingStationId);
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Lorem.Sentence(20).OfLength(1000));
        RuleFor(x => x.Description, f => f.Lorem.Sentence(100).OfLength(10000));
    }
}