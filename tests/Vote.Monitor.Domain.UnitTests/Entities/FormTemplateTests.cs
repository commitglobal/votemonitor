using System.Text.Json;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class FormTemplateTests
{
    [Fact]
    public void Should_DeserializeCorrectly_WhenPolymorphicQuestions()
    {
        var serialized = @"
        [
              {
                ""$questionType"": ""numberQuestion"",
                ""Id"": ""d519a2aa-4b68-4470-88a3-492fcf597df1"",
                ""Code"": ""C1"",
                ""Text"": {
                  ""RO"": ""How many PEC members have been appointed""
                },
                ""Helptext"": null,
                ""InputPlaceholder"": null
            }
        ]";

        var questions = JsonSerializer.Deserialize<IReadOnlyList<BaseQuestion>>(serialized);
        questions.Should().NotBeNullOrEmpty();
        questions.Should().HaveCount(1);
        questions!.First().Should().BeOfType<NumberQuestion>();
    }

    [Fact]
    public void Should_SerializeCorrectly_WhenPolymorphicQuestions()
    {
        var numberQuestion = new NumberQuestion(Guid.Parse("d519a2aa-4b68-4470-88a3-492fcf597df1"),
            "A code",
            new TranslatedString { ["RO"] = "A text" },
            new TranslatedString { ["RO"] = "A helptext" },
            new TranslatedString { ["RO"] = "A placeholder" });

        var serialized = JToken.Parse(JsonSerializer.Serialize(numberQuestion));

        var expected = JToken.Parse(@"{
            ""Id"": ""d519a2aa-4b68-4470-88a3-492fcf597df1"",
            ""Code"": ""A code"",
            ""Text"": {""RO"":""A text""},
            ""Helptext"": {""RO"":""A helptext""},
            ""InputPlaceholder"": {""RO"":""A placeholder""},
            ""$questionType"": ""numberQuestion""
        }");

        serialized.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_()
    {

    }
}
