using Vote.Monitor.Core.Models;
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
            new NumberQuestion(Guid.NewGuid(), "c1", numberQuestionText, null, null),
            new DateQuestion(Guid.NewGuid(), "c2", dateQuestionText, null),
            new TextQuestion(Guid.NewGuid(), "c3", textQuestionText, null, null),
            new RatingQuestion(Guid.NewGuid(), "c4", ratingQuestionText, null, RatingScale.OneTo10),
            new SingleSelectQuestion(Guid.NewGuid(), "c5", singleSelectQuestionText, null, singleSelectOptions),
            new MultiSelectQuestion(Guid.NewGuid(), "c6", multiSelectQuestionText, null, multiSelectOptions)
        ];

        CustomInstantiator(_ =>
        {
            var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "C1", new TranslatedString(),new TranslatedString(),
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
