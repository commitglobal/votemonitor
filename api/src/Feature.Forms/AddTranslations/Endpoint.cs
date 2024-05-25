using Authorization.Policies;
using Feature.Forms.Specifications;

namespace Feature.Forms.AddTranslations;

public class Endpoint(IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/forms/{id}:addTranslations");
        Summary(x =>
        {
            x.Description = "Adds supported languages to a form";
            x.Description = "For each new language a default value will be set in all form template. No duplicated translations will be added.";
        });
        Policies(PolicyNames.NgoAdminsOnly);
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var form = await repository.FirstOrDefaultAsync(
            specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        form.AddTranslations(req.LanguageCodes);
        await repository.UpdateAsync(form, ct);

        return TypedResults.NoContent();
    }
}
