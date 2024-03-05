using System.Text.Json;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class FormTemplateTests
{
    [Fact]
    public void Should_DeserializeCorrectly_WhenPolymorphicQuestions()
    {
        var serialized = @"
        [
          {
            ""Id"": ""aac972c6-50dc-48ef-8a3c-0d5d829cc1d2"",
            ""Code"": ""A"",
            ""Title"": {
              ""RO"": ""a section""
            },
            ""Questions"": [
              {
                ""$questionType"": ""numberInputQuestion"",
                ""Id"": ""d519a2aa-4b68-4470-88a3-492fcf597df1"",
                ""Code"": ""C1"",
                ""Text"": {
                  ""RO"": ""How many PEC members have been appointed""
                },
                ""Helptext"": null,
                ""InputPlaceholder"": null
              }
            ]
          }
        ]";

        var sections = JsonSerializer.Deserialize<IReadOnlyList<FormSection>>(serialized);
        sections.Should().NotBeNullOrEmpty();
        sections.Should().HaveCount(1);
        sections!.First().Questions.Should().HaveCount(1);
        var question = sections!.First().Questions.First();
        question.Should().BeOfType<NumberInputQuestion>();
    }

    [Fact]
    public void Should_SerializeCorrectly_WhenPolymorphicQuestions()
    {
        var numberInputQuestion = new NumberInputQuestion(Guid.Parse("d519a2aa-4b68-4470-88a3-492fcf597df1"),
            "A code",
            new TranslatedString { ["RO"] = "A text" },
            new TranslatedString { ["RO"] = "A helptext" },
            new TranslatedString { ["RO"] = "A placeholder" });

        var serialized = JToken.Parse(JsonSerializer.Serialize(numberInputQuestion));

        var expected = JToken.Parse(@"{
            ""Id"": ""d519a2aa-4b68-4470-88a3-492fcf597df1"",
            ""Code"": ""A code"",
            ""Text"": {""RO"":""A text""},
            ""Helptext"": {""RO"":""A helptext""},
            ""InputPlaceholder"": {""RO"":""A placeholder""},
            ""$questionType"": ""numberInputQuestion""
        }");

        serialized.Should().BeEquivalentTo(expected);
    }
}
