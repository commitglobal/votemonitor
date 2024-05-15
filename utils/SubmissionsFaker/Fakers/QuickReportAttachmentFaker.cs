using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class QuickReportAttachmentFaker : Faker<QuickReportAttachmentRequest>
{
    public QuickReportAttachmentFaker(List<QuickReportRequest> quickReports)
    {
        ;
        Rules((f, x) =>
        {
            var quickReport = f.PickRandom(quickReports);

            x.Id = f.Random.Guid();
            x.QuickReportId = quickReport.Id;
            x.ObserverToken = quickReport.ObserverToken;
        });
    }
}