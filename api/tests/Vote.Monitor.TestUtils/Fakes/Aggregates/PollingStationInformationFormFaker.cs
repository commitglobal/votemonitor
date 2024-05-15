using Vote.Monitor.Core.Constants;
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
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), true),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), true, true),
        ];

        SelectOption[] multiSelectOptions =
        [
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate()),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), true),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), translatedStringFaker.Generate(), true, true),
        ];

        questions ??=
        [
            NumberQuestion.Create(Guid.NewGuid(), "c1", numberQuestionText),
            DateQuestion.Create(Guid.NewGuid(), "c2", dateQuestionText),
            TextQuestion.Create(Guid.NewGuid(), "c3", textQuestionText),
            RatingQuestion.Create(Guid.NewGuid(), "c4", ratingQuestionText, RatingScale.OneTo10),
            SingleSelectQuestion.Create(Guid.NewGuid(), "c5", singleSelectQuestionText, singleSelectOptions),
            MultiSelectQuestion.Create(Guid.NewGuid(), "c6", multiSelectQuestionText, multiSelectOptions)
        ];

        CustomInstantiator(_ =>
            PollingStationInformationForm.Create(electionRound, languages.First(), languages, questions));
    }
}
