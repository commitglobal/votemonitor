using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.PollingStation.Information.Forms.Upsert;

public class Endpoint(
    IRepository<PollingStationInfoFormAggregate> repository,
    IRepository<ElectionRound> electionRoundRepository,
    IUserDisplayNameProvider userDisplayNames)
    : Endpoint<Request, Results<Ok<PollingStationInformationFormModel>, NotFound,
        Conflict<VersionConflict<PollingStationInformationFormModel>>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-station-information-form");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PollingStationInformationFormModel>, NotFound,
        Conflict<VersionConflict<PollingStationInformationFormModel>>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formSpecification = new GetPollingStationInformationFormSpecification(req.ElectionRoundId);
        var form = await repository.FirstOrDefaultAsync(formSpecification, ct);
        var questions = req.Questions.Select(QuestionsMapper.ToEntity).ToList();

        if (form is null)
        {
            return await AddPollingStationInfoFormAsync(req.ElectionRoundId, req.DefaultLanguage, req.Languages,
                questions, ct);
        }

        if (RecordVersions.IsStale(req.Version, form.Version))
        {
            return await ConflictAsync(req.ElectionRoundId, ct);
        }

        form.UpdateDetails("PSI", TranslatedString.New(req.Languages, "PSI"),
            TranslatedString.New(req.Languages, "PSI"), FormType.PSI, req.DefaultLanguage, req.Languages, null,
            questions);

        try
        {
            await repository.UpdateAsync(form, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Saved by someone else between reading and writing it.
            return await ConflictAsync(req.ElectionRoundId, ct);
        }

        return TypedResults.Ok(await PollingStationInformationFormModel.FromEntity(form)
            .WithLastModifiedByNameAsync(userDisplayNames, ct));
    }

    private async Task<Results<Ok<PollingStationInformationFormModel>, NotFound,
        Conflict<VersionConflict<PollingStationInformationFormModel>>>> ConflictAsync(Guid electionRoundId,
        CancellationToken ct)
    {
        var current = await repository.FirstOrDefaultAsync(
            new PollingStationInformationModelSpecification(electionRoundId), ct);
        if (current is null)
        {
            return TypedResults.NotFound();
        }

        var model = await current.WithLastModifiedByNameAsync(userDisplayNames, ct);
        return TypedResults.Conflict(VersionConflict<PollingStationInformationFormModel>.Of(model));
    }

    private async Task<Results<Ok<PollingStationInformationFormModel>, NotFound,
        Conflict<VersionConflict<PollingStationInformationFormModel>>>> AddPollingStationInfoFormAsync(
        Guid electionRoundId,
        string defaultLanguage,
        List<string> languages,
        List<BaseQuestion> questions,
        CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(electionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        var pollingStationInformationForm =
            PollingStationInfoFormAggregate.Create(electionRound, defaultLanguage, languages, questions);
        await repository.AddAsync(pollingStationInformationForm, ct);

        return TypedResults.Ok(await PollingStationInformationFormModel.FromEntity(pollingStationInformationForm)
            .WithLastModifiedByNameAsync(userDisplayNames, ct));
    }
}
