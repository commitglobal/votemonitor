using Authorization.Policies;
using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(IReadRepository<FormSubmission> repository, IReadRepository<FormAggregate> formRepository) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byForm");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x =>
        {
            x.Summary = "Form submissions aggregated by observer";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var submissions = await repository.ListAsync(new ListFormSubmissions(req), ct);

        var forms = await formRepository.ListAsync(new ListNgoForms(req.ElectionRoundId, req.NgoId), ct);
        var submissionsByFormId = submissions
            .GroupBy(x => x.FormId, y => y, (formId, g) => new { FormId = formId, Submissions = g.ToList() })
            .ToDictionary(x => x.FormId, y => y.Submissions);

        List<FormSubmissionsAggregate> aggregates = [];
        foreach (var form in forms)
        {
            var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
            foreach (var formSubmission in submissionsByFormId[form.Id])
            {
                formSubmissionsAggregate.AggregateAnswers(formSubmission);
            }

            aggregates.Add(formSubmissionsAggregate);
        }

        return new Response() { FormSubmissionsAggregates = aggregates, };
    }
}
