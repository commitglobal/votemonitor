using Authorization.Policies;
using Feature.Forms.Specifications;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.DeleteTranslation;

public class Endpoint(IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/forms/{id}/{languageCode}");
        Description(x => x.Accepts<Request>());
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Description = "Removes supported language from a form";
            x.Description = "Translations for removed language will be removed as well.";
        });
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.Id);
        var form = await formsRepository.FirstOrDefaultAsync(specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        form.RemoveTranslation(req.LanguageCode);
        await formsRepository.UpdateAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();

    }
}
