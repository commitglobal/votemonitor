using Bogus;
using SubmissionsFaker.Clients.Citizen.Models;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class CitizenReportNoteFaker : Faker<CitizenReportNoteRequest>
{
    public CitizenReportNoteFaker(List<CitizenReportRequest> citizenReports)
    {
        Rules((f, x) =>
        {
            var citizenReportRequest = f.PickRandom(citizenReports);

            x.Id = f.Random.Guid();
            x.CitizenReportId = citizenReportRequest.CitizenReportId;
            x.FormId = citizenReportRequest.FormId;
            x.QuestionId = f.PickRandom(citizenReportRequest.Answers).QuestionId;
            x.Text = f.Lorem.Sentence(1000).Substring(0, 1000);
        });
    }
}