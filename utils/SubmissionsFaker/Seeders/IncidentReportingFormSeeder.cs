using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class IncidentReportingFormSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        LoginResponse ngoAdminToken,
        string electionRoundId,
        ProgressTask progressTask)
    {
        progressTask.MaxValue(1);

        progressTask.StartTask();
        var incidentReportingForm = await ngoAdminApi.CreateForm(electionRoundId,
            IncidentReportingFormData.IncidentReporting with { FormType = "IncidentReporting" }, ngoAdminToken.Token);
        await ngoAdminApi.UpdateForm(electionRoundId, incidentReportingForm.Id, IncidentReportingFormData.IncidentReporting,
            ngoAdminToken.Token);
        await ngoAdminApi.PublishForm(electionRoundId, incidentReportingForm.Id, ngoAdminToken.Token);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = incidentReportingForm.Id,
                Questions = IncidentReportingFormData.IncidentReporting.Questions,
            },
            // ...
        ];
    }
}