using Bogus;
using SubmissionsFaker.Clients.Citizen.Models;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class CitizenReportAttachmentFaker : Faker<CitizenReportAttachmentRequest>
{
    public CitizenReportAttachmentFaker(List<CitizenReportRequest> citizenReports)
    {
        Rules((f, x) =>
        {
            var citizenReport = f.PickRandom(citizenReports);

            x.CitizenReportId = citizenReport.CitizenReportId;
            x.Id = f.Random.Guid();
            x.FormId = citizenReport.FormId;
            x.QuestionId = f.PickRandom(citizenReport.Answers).QuestionId;
        });
    }
}