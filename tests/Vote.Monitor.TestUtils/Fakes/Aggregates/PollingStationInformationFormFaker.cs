using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class PollingStationInformationFormFaker : PrivateFaker<PollingStationInformationForm>
{
    public PollingStationInformationFormFaker(ElectionRoundAggregate? electionRound = null,
        List<string>? languages = null,
        List<BaseQuestion>? questions = null)
    {
        languages ??= [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1];
        electionRound ??= new ElectionRoundAggregateFaker().Generate();

        var translatedStringFaker = new TranslatedStringFaker(languages);

        var numberQuestionText = translatedStringFaker.Generate();
        var dateQuestionText = translatedStringFaker.Generate();
        var textQuestionText = translatedStringFaker.Generate();
        var ratingQuestionText = translatedStringFaker.Generate();
        var singleSelectQuestionText = translatedStringFaker.Generate();
        var multiSelectQuestionText = translatedStringFaker.Generate();

        SelectOption[] singleSelectOptions =
        [
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), false, false),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), true, false),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), false, true),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), true, true),
        ];

        SelectOption[] multiSelectOptions =
        [
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), false, false),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), true, false),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), false, true),
            new SelectOption(Guid.NewGuid(), translatedStringFaker.Generate(), true, true),
        ];

        questions ??=
        [
            new NumberQuestion(Guid.NewGuid(), "c1", numberQuestionText, null, null),
            new DateQuestion(Guid.NewGuid(), "c2", dateQuestionText, null),
            new TextQuestion(Guid.NewGuid(), "c3", textQuestionText, null, null),
            new RatingQuestion(Guid.NewGuid(), "c4", ratingQuestionText, null, RatingScale.OneTo10),
            new SingleSelectQuestion(Guid.NewGuid(), "c5", singleSelectQuestionText, null, singleSelectOptions),
            new MultiSelectQuestion(Guid.NewGuid(), "c6", multiSelectQuestionText, null, multiSelectOptions)
        ];
        CustomInstantiator(_ =>
            PollingStationInformationForm.Create(electionRound, languages, questions));
    }
}
