using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.UnitTests;

public class TranslatedStringTests
{
    [Fact]
    public void ComparingToATranslatedString_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var translatedString1 = new TranslatedString();
        translatedString1.Add("EN", "Some text");
        translatedString1.Add("RO", "Other text");

        var translatedString2 = new TranslatedString();
        translatedString2.Add("RO", "Other text");
        translatedString2.Add("EN", "Some text");

        // Act
        var result = translatedString1.Equals(translatedString2);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(DifferentTranslatedStringsTestData))]
    public void ComparingToATranslatedString_WithDifferentProperties_ReturnsFalse(TranslatedString string1, TranslatedString string2)
    {
        // Act
        var result = string1.Equals(string2);

        // Assert
        result.Should().BeFalse();
    }

    public static IEnumerable<object[]> DifferentTranslatedStringsTestData =>
        new List<object[]>
        {
            new object[] { new TranslatedString { { "EN", "Some text" }, { "RO", "Other text" } }, new TranslatedString { { "EN", "Some text" }, { "RO", "Other different text" } } },
            new object[] { new TranslatedString { { "EN", "Some text" } }, new TranslatedString { { "EN", "Some text" }, { "RO", "Other different text" } } },
            new object[] {  new TranslatedString { { "EN", "Some text" }, { "RO", "Other text" } }, new TranslatedString { { "EN", "Some text" } }  },
            new object[] {  new TranslatedString { { "EN", "Some text" } }, new TranslatedString { { "RO", "Other different text" } }  }
        };

}
