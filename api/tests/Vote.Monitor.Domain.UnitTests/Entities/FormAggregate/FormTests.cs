using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    private static readonly string[] _languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1];

    private readonly TranslatedString _name = new TranslatedStringFaker(_languages).Generate();
    private readonly TranslatedString _description = new TranslatedStringFaker(_languages).Generate();
    
    private readonly TextQuestion _textQuestion = new TextQuestionFaker(_languages).Generate();
    private readonly NumberQuestion _numberQuestion = new NumberQuestionFaker(_languages).Generate();
    private readonly DateQuestion _dateQuestion = new DateQuestionFaker(_languages).Generate();
    private readonly RatingQuestion _ratingQuestion = new RatingQuestionFaker(languageList: _languages).Generate();

    private readonly SingleSelectQuestion _singleSelectQuestion =
        new SingleSelectQuestionFaker(languageList: _languages).Generate();

    private readonly MultiSelectQuestion _multiSelectQuestion =
        new MultiSelectQuestionFaker(languageList: _languages).Generate();
}
