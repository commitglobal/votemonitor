using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    private static readonly string[] Languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1];

    private readonly TranslatedString _name = new TranslatedStringFaker(Languages).Generate();
    private readonly TranslatedString _description = new TranslatedStringFaker(Languages).Generate();
}