using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Module.Forms.Requests;
using Vote.Monitor.TestUtils.Utils;

namespace Module.Forms.UnitTests.Validators;

public class ValidatorsTestData
{
    public static IEnumerable<object[]> InvalidPartiallyTranslatedTestCases =>
        InvalidPartiallyTranslatedTestData
            .Select(x => new object[] { x })
            .ToList();

    public static IEnumerable<TranslatedString> InvalidPartiallyTranslatedTestData =>
    [
        new TranslatedString
        {
            [LanguagesList.IT.Iso1] = "an italian string"
        },
        new TranslatedString { [""] = "an empty" },
        new TranslatedString { ["aaa"] = "an invalid iso" },
        new TranslatedString { ["a"] = "an invalid iso" },
        new TranslatedString()
    ];

    public static IEnumerable<object[]> ValidPartiallyTranslatedTestCases =>
        ValidPartiallyTranslatedTestData
            .Select(x => new object[] { x })
            .ToList();

    public static IEnumerable<TranslatedString> ValidPartiallyTranslatedTestData =>
    [
        new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "english string",
            [LanguagesList.RO.Iso1] = ""
        },
        new TranslatedString
        {
            [LanguagesList.RO.Iso1] = "",
            [LanguagesList.EN.Iso1] = "english string"
        },
        new TranslatedString
        {
            [LanguagesList.EN.Iso1] = "english string",
            [LanguagesList.RO.Iso1] = "romanian string"
        }
    ];

    public static IEnumerable<object[]> InvalidCodeTestCases =>
        new List<object[]>
        {
            new object[] { "a".Repeat(257) }
        };

    public static IEnumerable<object?[]> ValidDisplayLogicTestCases =>
        new List<object?[]>
        {
            new object?[] { new DisplayLogicRequest{ParentQuestionId= Guid.NewGuid(), Condition = DisplayLogicCondition.GreaterEqual, Value = "1"} },
            new object?[] { null }
        };


}
