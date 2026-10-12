using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Core.UnitTests.HelpersTests;

public class PersonalDataMaskerTests
{
    [Theory]
    [InlineData("ion.dormenco@gmail.com", "i**********o@gmail.com")]
    [InlineData("io@gmail.com", "i*@gmail.com")]
    [InlineData("i@gmail.com", "*@gmail.com")]
    [InlineData("not-an-email", "n**********l")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void MaskEmail_ShouldKeepFirstAndLastCharacterOfLocalPart(string? email, string expected)
    {
        PersonalDataMasker.MaskEmail(email).Should().Be(expected);
    }

    [Theory]
    [InlineData("Ion", "I*n")]
    [InlineData("Dormenco", "D******o")]
    [InlineData("Ion Dormenco", "I*n D******o")]
    [InlineData("  Ion   Dormenco ", "I*n D******o")]
    [InlineData("Al", "A*")]
    [InlineData("A", "*")]
    [InlineData(" ", "")]
    [InlineData(null, "")]
    public void MaskName_ShouldKeepFirstAndLastCharacterOfEachWord(string? name, string expected)
    {
        PersonalDataMasker.MaskName(name).Should().Be(expected);
    }

    [Theory]
    [InlineData("+40712345678", "**********78")]
    [InlineData("12", "**")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void MaskPhoneNumber_ShouldKeepOnlyLastTwoCharacters(string? phoneNumber, string expected)
    {
        PersonalDataMasker.MaskPhoneNumber(phoneNumber).Should().Be(expected);
    }
}
