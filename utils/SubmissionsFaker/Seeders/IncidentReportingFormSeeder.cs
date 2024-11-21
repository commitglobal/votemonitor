using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class IncidentReportingFormSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        Guid electionRoundId,
        ProgressTask progressTask)
    {
        progressTask.MaxValue(1);

        progressTask.StartTask();
        var form = IncidentReportingFormData.IncidentReporting("IR");
        var incidentReportingForm = await ngoAdminApi.CreateForm(electionRoundId, form);
        await ngoAdminApi.PublishForm(electionRoundId, incidentReportingForm.Id);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = incidentReportingForm.Id,
                Questions = form.Questions,
            },
            // ...
        ];
    }
}