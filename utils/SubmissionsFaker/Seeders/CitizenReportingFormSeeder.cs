using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class CitizenReportingFormSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        Guid electionRoundId,
        ProgressTask progressTask)
    {
        progressTask.MaxValue(1);

        progressTask.StartTask();
        var form = CitizenReportingFormData.CitizenReporting("CR");
        var citizenReporting = await ngoAdminApi.CreateForm(electionRoundId, form);
        await ngoAdminApi.PublishForm(electionRoundId, citizenReporting.Id);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = citizenReporting.Id,
                Questions = form.Questions,
            },
            // ...
        ];
    }
}