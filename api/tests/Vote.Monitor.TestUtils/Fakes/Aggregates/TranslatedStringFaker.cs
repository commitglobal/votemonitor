using Bogus;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class TranslatedStringFaker : Faker<TranslatedString>
{
    public TranslatedStringFaker(List<string>? languages = null)
    {
        languages ??= FakerHub.PickRandom(LanguagesList.GetAll().Select(x => x.Iso1), 3).ToList();
        TranslatedString translatedString = [];

        foreach (var language in languages)
        {
            translatedString[language] = FakerHub.Lorem.Sentence();
        }

        CustomInstantiator(_ => translatedString);
    }
}
