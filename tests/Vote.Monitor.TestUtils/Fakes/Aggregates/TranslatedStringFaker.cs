using Bogus;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class TranslatedStringFaker : Faker<TranslatedString>
{
    public TranslatedStringFaker(List<string> languages)
    {
        TranslatedString translatedString = [];

        foreach (var language in languages)
        {
            translatedString[language] = FakerHub.Lorem.Sentence();
        }

        CustomInstantiator(_ => translatedString);
    }
}
