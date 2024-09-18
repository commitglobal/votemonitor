using Bogus;
using SubmissionsFaker.Clients.Citizen.Models;
using SubmissionsFaker.Clients.Locations;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class CitizenReportsFaker : Faker<CitizenReportRequest>
{
    public CitizenReportsFaker(List<UpdateFormResponse> forms, List<LocationNode> locations)
    {
        Rules((f, x) =>
        {
            var form = f.PickRandom(forms);
            
            x.CitizenReportId = f.Random.Guid().ToString();
            x.FormId = form.Id;
            x.LocationId = f.PickRandom(locations).LocationId!.Value;
            
            x.Answers = f.PickRandom(form.Questions, f.Random.Int(0, form.Questions.Count))
                .Select(Answers.GetFakeAnswer).ToList();
        });
    }
}