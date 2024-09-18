using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.PollingStation.Information.Form.Upsert;

public class Endpoint(
    IRepository<PollingStationInfoFormAggregate> repository,
    IRepository<ElectionRound> electionRoundRepository)
    : Endpoint<Request, Results<Ok<PollingStationInformationFormModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-station-information-form");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PollingStationInformationFormModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var formSpecification = new GetPollingStationInformationFormSpecification(req.ElectionRoundId);
        var form = await repository.FirstOrDefaultAsync(formSpecification, ct);
        var questions = req.Questions.Select(QuestionsMapper.ToEntity).ToList();

        return form is null
            ? await AddPollingStationInfoFormAsync(req.ElectionRoundId, req.DefaultLanguage, req.Languages, questions,
                ct)
            : await UpdateForm(form, req.DefaultLanguage, req.Languages, questions, ct);
    }

    private async Task<Results<Ok<PollingStationInformationFormModel>, NotFound>> UpdateForm(
        PollingStationInfoFormAggregate pollingStationInformationForm,
        string defaultLanguage,
        List<string> languages,
        List<BaseQuestion> questions,
        CancellationToken ct)
    {
        pollingStationInformationForm.UpdateDetails("PSI", TranslatedString.New(languages, "PSI"),
            TranslatedString.New(languages, "PSI"), FormType.PSI, defaultLanguage, languages, questions);
        await repository.UpdateAsync(pollingStationInformationForm, ct);

        return TypedResults.Ok(PollingStationInformationFormModel.FromEntity(pollingStationInformationForm));
    }

    private async Task<Results<Ok<PollingStationInformationFormModel>, NotFound>> AddPollingStationInfoFormAsync(
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

        return TypedResults.Ok(PollingStationInformationFormModel.FromEntity(pollingStationInformationForm));
    }
}