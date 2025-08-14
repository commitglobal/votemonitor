using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.ElectionRounds.Create;

public class Endpoint(
    IRepository<ElectionRoundAggregate> repository,
    IRepository<PollingStationInformationForm> psiFormsRepository)
    : Endpoint<Request, Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetActiveElectionRoundSpecification(req.CountryId, req.Title);
        var hasElectionRoundWithSameTitle = await repository.AnyAsync(specification, ct);

        if (hasElectionRoundWithSameTitle)
        {
            AddError(r => r.Title, "An election round with same title already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = new ElectionRoundAggregate(req.CountryId, req.Title, req.EnglishTitle, req.StartDate);
        await repository.AddAsync(electionRound, ct);
        
        IEnumerable<string> languages = new[] { LanguagesList.EN.Iso1 };
        var option1 = SelectOption.Create(Guid.NewGuid(), TranslatedString.New(languages, "Yes"));
        var option2 = SelectOption.Create(Guid.NewGuid(), TranslatedString.New(languages, "Yes!"));
        var questions = new BaseQuestion[]
        {
            SingleSelectQuestion.Create(Guid.NewGuid(), "PSI",
                TranslatedString.New(languages, "Did you forgot to add PSI questions?"), [option1, option2])
        };
        var psiForm = PollingStationInformationForm.Create(electionRound, LanguagesList.EN.Iso1, languages, questions);
        await psiFormsRepository.AddAsync(psiForm, ct);

        var country = CountriesList.Get(req.CountryId)!;

        return TypedResults.Ok(new ElectionRoundModel
        {
            Id = electionRound.Id,
            Title = electionRound.Title,
            EnglishTitle = electionRound.EnglishTitle,
            StartDate = electionRound.StartDate,
            Status = electionRound.Status,
            CreatedOn = electionRound.CreatedOn,
            LastModifiedOn = electionRound.LastModifiedOn,
            CountryId = req.CountryId,
            CountryIso2 = country.Iso2,
            CountryIso3 = country.Iso3,
            CountryName = country.Name,
            CountryFullName = country.FullName,
            CountryNumericCode = country.NumericCode,
            CoalitionId = null,
            CoalitionName = null,
            IsCoalitionLeader = false,
            IsMonitoringNgoForCitizenReporting = false,
            AllowMultipleFormSubmission =null
        });
    }
}
