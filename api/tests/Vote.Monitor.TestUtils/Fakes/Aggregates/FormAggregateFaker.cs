using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class FormAggregateFaker : PrivateFaker<Form>
{
    public FormAggregateFaker(ElectionRoundAggregate? electionRound = null,
        MonitoringNgo? monitoringNgo = null,
        List<string>? languages = null,
        List<BaseQuestion>? questions = null,
        FormStatus? status = null)
    {
        languages ??= [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1];
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        monitoringNgo ??= new MonitoringNgoAggregateFaker(electionRound: electionRound).Generate();

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
            NumberQuestion.Create(Guid.NewGuid(), "c1", numberQuestionText),
            DateQuestion.Create(Guid.NewGuid(), "c2", dateQuestionText),
            TextQuestion.Create(Guid.NewGuid(), "c3", textQuestionText),
            RatingQuestion.Create(Guid.NewGuid(), "c4", ratingQuestionText, RatingScale.OneTo10),
            SingleSelectQuestion.Create(Guid.NewGuid(), "c5", singleSelectQuestionText, singleSelectOptions),
            MultiSelectQuestion.Create(Guid.NewGuid(), "c6", multiSelectQuestionText, multiSelectOptions)
        ];

        CustomInstantiator(_ =>
        {
            var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "C1", translatedStringFaker.Generate(), translatedStringFaker.Generate(),
                languages.First(), languages, questions);

            if (status == FormStatus.Obsolete)
            {
                form.Obsolete();
            }

            if (status == FormStatus.Published)
            {
                form.Publish();
            }

            if (status == FormStatus.Drafted)
            {
                form.Draft();
            }

            return form;
        });
    }
}
