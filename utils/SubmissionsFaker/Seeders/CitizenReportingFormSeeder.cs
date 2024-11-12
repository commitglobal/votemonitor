using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class CitizenReportingFormSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        LoginResponse ngoAdminToken,
        string electionRoundId,
        ProgressTask progressTask)
    {
        progressTask.MaxValue(1);

        progressTask.StartTask();
        var citizenReporting = await ngoAdminApi.CreateForm(electionRoundId,
            CitizenReportingFormData.CitizenReporting with { FormType = "CitizenReporting" }, ngoAdminToken.Token);
        await ngoAdminApi.UpdateForm(electionRoundId, citizenReporting.Id, CitizenReportingFormData.CitizenReporting,
            ngoAdminToken.Token);
        await ngoAdminApi.PublishForm(electionRoundId, citizenReporting.Id, ngoAdminToken.Token);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = citizenReporting.Id,
                Questions = CitizenReportingFormData.CitizenReporting.Questions,
            },
            // ...
        ];
    }
}