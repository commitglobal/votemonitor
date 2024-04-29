using Spectre.Console;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class FormsSeeder
{
    public static async Task<List<CreateResponse>> Seed(INgoAdminApi ngoAdminApi,
        LoginResponse ngoAdminToken,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ProgressTask progressTask)
    {
        progressTask.StartTask();

        var openingForm = await ngoAdminApi.CreateForm(electionRoundId, monitoringNgoId, FormData.OpeningForm, ngoAdminToken.Token);
        await ngoAdminApi.UpdateForm(electionRoundId, monitoringNgoId, openingForm.Id, FormData.OpeningForm, ngoAdminToken.Token);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();
        return [openingForm];
    }
}