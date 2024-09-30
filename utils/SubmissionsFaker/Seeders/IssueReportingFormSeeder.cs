using Spectre.Console;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Forms;

namespace SubmissionsFaker.Seeders;

public class IssueReportingFormSeeder
{
    public static async Task<List<UpdateFormResponse>> Seed(INgoAdminApi ngoAdminApi,
        LoginResponse ngoAdminToken,
        string electionRoundId,
        ProgressTask progressTask)
    {
        progressTask.MaxValue(1);

        progressTask.StartTask();
        var openingForm = await ngoAdminApi.CreateForm(electionRoundId,
            IssueReportingFormData.IssueReporting with { FormType = "IssueReporting" }, ngoAdminToken.Token);
        await ngoAdminApi.UpdateForm(electionRoundId, openingForm.Id, IssueReportingFormData.IssueReporting,
            ngoAdminToken.Token);
        await ngoAdminApi.PublishForm(electionRoundId, openingForm.Id, ngoAdminToken.Token);
        progressTask.Increment(1);

        progressTask.Increment(progressTask.MaxValue);
        progressTask.StopTask();

        return
        [
            new UpdateFormResponse
            {
                Id = openingForm.Id,
                Questions = IssueReportingFormData.IssueReporting.Questions,
            },
            // ...
        ];
    }
}