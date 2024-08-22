namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public class FormTestsTestData
{
    static FormTestsTestData()
    {
        PartiallyTranslatedRatingQuestion = RatingQuestion.Create(Guid.NewGuid(), "q", Text, RatingScale.OneTo3, Helptext, LowerLabel,
            UpperLabel);
    }

    private static readonly TranslatedString Text = new()
    {
        [LanguagesList.EN.Iso1] = "english question text",
        [LanguagesList.RO.Iso1] = string.Empty
    };

    private static readonly TranslatedString Helptext = new()
    {
        [LanguagesList.EN.Iso1] = "english question helptext",
        [LanguagesList.RO.Iso1] = string.Empty
    };

    private static readonly TranslatedString Placeholder = new()
    {
        [LanguagesList.EN.Iso1] = "english question helptext",
        [LanguagesList.RO.Iso1] = string.Empty
    };

    private static readonly TranslatedString Option1Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option1",
        [LanguagesList.RO.Iso1] = "romanian text for option1"
    };

    private static readonly TranslatedString Option2Text = new()
    {
        [LanguagesList.EN.Iso1] = "english text for option2",
        [LanguagesList.RO.Iso1] = string.Empty
    };

    private static readonly TranslatedString LowerLabel = new()
    {
        [LanguagesList.EN.Iso1] = "english text for lower label",
        [LanguagesList.RO.Iso1] = string.Empty
    };

    private static readonly TranslatedString UpperLabel = new()
    {
        [LanguagesList.EN.Iso1] = "english text for upper label",
        [LanguagesList.RO.Iso1] = string.Empty
    };


    private static readonly SelectOption[] Options =
    [
        new(Guid.NewGuid(), Option1Text, false, false),
        new(Guid.NewGuid(), Option2Text, false, true),
    ];

    private static readonly TextQuestion PartiallyTranslatedTextQuestion =
        TextQuestion.Create(Guid.NewGuid(), "q", Text, Helptext, Placeholder);

    private static readonly NumberQuestion PartiallyTranslatedNumberQuestion =
        NumberQuestion.Create(Guid.NewGuid(), "q", Text, Helptext, Placeholder);

    private static readonly DateQuestion PartiallyTranslatedDateQuestion = DateQuestion.Create(Guid.NewGuid(), "q", Text, Helptext);
    private static readonly RatingQuestion PartiallyTranslatedRatingQuestion;

    private static readonly SingleSelectQuestion PartiallyTranslatedSingleSelectQuestion =
        SingleSelectQuestion.Create(Guid.NewGuid(), "q", Text, Options, Helptext);

    private static readonly MultiSelectQuestion PartiallyTranslatedMultiSelectQuestion =
        MultiSelectQuestion.Create(Guid.NewGuid(), "q", Text, Options, Helptext);

    public static IEnumerable<object[]> PartiallyTranslatedQuestionsTestData =>
        new List<object[]>
        {
            new object[] { new BaseQuestion[] { PartiallyTranslatedTextQuestion } },
            new object[] { new BaseQuestion[] { PartiallyTranslatedNumberQuestion } },
            new object[] { new BaseQuestion[] { PartiallyTranslatedDateQuestion } },
            new object[] { new BaseQuestion[] { PartiallyTranslatedRatingQuestion } },
            new object[] { new BaseQuestion[] { PartiallyTranslatedSingleSelectQuestion } },
            new object[] { new BaseQuestion[] { PartiallyTranslatedMultiSelectQuestion } },
            new object[]
            {
                new BaseQuestion[]
                {
                    PartiallyTranslatedTextQuestion,
                    PartiallyTranslatedNumberQuestion,
                    PartiallyTranslatedDateQuestion,
                    PartiallyTranslatedRatingQuestion,
                    PartiallyTranslatedSingleSelectQuestion,
                    PartiallyTranslatedMultiSelectQuestion
                }
            },
        };
}