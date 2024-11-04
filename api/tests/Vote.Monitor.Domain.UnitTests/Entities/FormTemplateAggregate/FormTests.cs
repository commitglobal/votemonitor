using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormTemplateAggregate;

public partial class FormTests
{
    private static readonly string[] _languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
    
    private readonly TextQuestion _textQuestion = new TextQuestionFaker(_languages).Generate();
    private readonly NumberQuestion _numberQuestion = new NumberQuestionFaker(_languages).Generate();
    private readonly DateQuestion _dateQuestion = new DateQuestionFaker(_languages).Generate();
    private readonly RatingQuestion _ratingQuestion = new RatingQuestionFaker(languageList: _languages).Generate();

    private readonly SingleSelectQuestion _singleSelectQuestion =
        new SingleSelectQuestionFaker(languageList: _languages).Generate();

    private readonly MultiSelectQuestion _multiSelectQuestion =
        new MultiSelectQuestionFaker(languageList: _languages).Generate();
}
