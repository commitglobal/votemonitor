using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.Token;

namespace SubmissionsFaker.Fakers;

public sealed class QuickReportAttachmentFaker : Faker<QuickReportAttachmentRequest>
{
    public QuickReportAttachmentFaker(List<QuickReportRequest> quickReports, List<LoginResponse> observers)
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.QuickReportId, f => f.PickRandom(quickReports).Id);
        RuleFor(x => x.ObserverToken, f => f.PickRandom(observers).Token);
    }
}