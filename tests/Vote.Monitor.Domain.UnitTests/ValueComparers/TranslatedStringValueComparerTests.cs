using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ValueComparers;

namespace Vote.Monitor.Domain.UnitTests.ValueComparers;

public class TranslatedStringValueComparerTests
{
    private readonly TranslatedStringValueComparer _valueComparer = new TranslatedStringValueComparer();

    [Fact]
    public void ComparingTwoTranslatedStrings_ShouldReturnTrue()
    {
        // Arrange
        var translatedString1 = new TranslatedString();
        translatedString1.Add("EN", "Some text");
        translatedString1.Add("RO", "Other text");

        var translatedString2 = new TranslatedString();
        translatedString2.Add("RO", "Other text");
        translatedString2.Add("EN", "Some text");

        // Act
        var result = _valueComparer.Equals(translatedString1, translatedString2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingTwoDifferentQuestionsLists_ShouldReturnFalse()
    {
        // Arrange
        var translatedString1 = new TranslatedString { { "EN", "Some text" }, { "RO", "Other text" } };

        var translatedString2 = new TranslatedString { { "RO", "Other text" }, { "EN", "Some different text" } };

        // Act
        var result = _valueComparer.Equals(translatedString1, translatedString2);

        // Assert
        result.Should().BeFalse();
    }
}
