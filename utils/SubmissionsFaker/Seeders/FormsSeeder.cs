using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class FormsSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        Guid electionRoundId,
        ProgressTask progressTask)
    {
        progressTask
            .MaxValue(2)
            .StartTask();

        var openingForm = ObservationForms.OpeningForm("Opening");
        var form = await ngoAdminApi.CreateForm(electionRoundId, openingForm);
        await ngoAdminApi.PublishForm(electionRoundId, form.Id);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = form.Id,
                Questions = openingForm.Questions
            },
            // ...
        ];
    }
}