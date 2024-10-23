using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.UnitTests;

public class LanguagesTranslationStatusTests
{
    private readonly LanguagesTranslationStatus _first = new LanguagesTranslationStatus
    {
        ["Ro"] = TranslationStatus.Translated,
        ["En"] = TranslationStatus.MissingTranslations,
    };

    private readonly LanguagesTranslationStatus _second = new LanguagesTranslationStatus
    {
        ["Ro"] = TranslationStatus.Translated,
        ["En"] = TranslationStatus.MissingTranslations,
    };


    [Fact]
    public void ShouldHaveSameHashCode_WhenDataIsTheSame()
    {
        _first.GetHashCode().Should().Be(_second.GetHashCode());
    }

    [Fact]
    public void ShouldBeEqual_WhenDataIsTheSame()
    {
        _first.Equals(_second).Should().BeTrue();
    }
}