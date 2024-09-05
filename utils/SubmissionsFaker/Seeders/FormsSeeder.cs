using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class FormsSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        LoginResponse ngoAdminToken,
        string electionRoundId,
        ProgressTask progressTask)
    {
        progressTask
            .MaxValue(2)
            .StartTask();

        var form = await ngoAdminApi.CreateForm(electionRoundId, FormData.OpeningForm, ngoAdminToken.Token);
        await ngoAdminApi.UpdateForm(electionRoundId, form.Id, FormData.OpeningForm, ngoAdminToken.Token);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = form.Id,
                Questions = FormData.OpeningForm.Questions
            },
            // ...
        ];
    }
}