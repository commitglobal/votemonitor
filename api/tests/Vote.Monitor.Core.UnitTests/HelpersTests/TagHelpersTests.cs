using System.Text.Json;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.UnitTests.HelpersTests;

public class TagHelpersTests
{
    [Fact]
    public void ToTagsObject_Dictionary_ReturnsExpectedJsonDocument()
    {
        // Arrange
        var tags = new Dictionary<string, string>
        {
            { "Tag1", "Value1" },
            { "Tag2", "Value2" }
        };

        // Act
        var jsonDocument = tags.ToTagsObject();

        // Assert
        var expectedJson = "{\"Tag1\":\"Value1\",\"Tag2\":\"Value2\"}";
        jsonDocument.RootElement.GetRawText().Should().Be(expectedJson);
    }

    [Fact]
    public void ToTagsObject_TagImportModelList_ReturnsExpectedJsonDocument()
    {
        // Arrange
        var tags = new List<TagImportModel>
        {
            new() { Name = "Tag1", Value = "Value1" },
            new() { Name = "Tag2", Value = "Value2" }
        };

        // Act
        var jsonDocument = tags.ToTagsObject();

        // Assert
        var expectedJson = "{\"Tag1\":\"Value1\",\"Tag2\":\"Value2\"}";
        jsonDocument.RootElement.GetRawText().Should().Be(expectedJson);
    }

    [Fact]
    public void ToDictionary_JsonDocument_ReturnsExpectedDictionary()
    {
        // Arrange
        var jsonDocument = JsonDocument.Parse("{\"Tag1\":\"Value1\",\"Tag2\":\"Value2\"}");

        // Act
        var dictionary = jsonDocument.ToDictionary();

        // Assert
        var expectedDictionary = new Dictionary<string, string>
        {
            { "Tag1", "Value1" },
            { "Tag2", "Value2" }
        };
        dictionary.Should().BeEquivalentTo(expectedDictionary);
    }
}
