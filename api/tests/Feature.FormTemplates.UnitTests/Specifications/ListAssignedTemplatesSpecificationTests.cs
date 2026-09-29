using Feature.FormTemplates.ListAssignedTemplates;
using Feature.FormTemplates.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.UnitTests.Specifications;

public class ListAssignedTemplatesSpecificationTests
{
    [Fact]
    public void ListAssignedTemplatesSpecification_Should_Filter_By_ElectionRoundId_And_Return_Correct_FormTemplates()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();
        var request = new Request
        {
            ElectionRoundId = electionRoundId
        };

        var matchingTemplate = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var secondMatchingTemplate = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var nonMatchingTemplate = new ElectionRoundFormTemplateAggregateFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var testCollection = new List<ElectionRoundFormTemplate>
        {
            matchingTemplate,
            secondMatchingTemplate,
            nonMatchingTemplate
        }.AsQueryable();

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        var expectedIds = new List<Guid> { matchingTemplate.FormTemplate.Id, secondMatchingTemplate.FormTemplate.Id };

        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().BeEquivalentTo(expectedIds);
    }

    [Fact]
    public void ListAssignedTemplatesSpecification_Should_Return_All_Matching_Templates_Ordered_By_Code()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();

        var testCollection = Enumerable
            .Range(1, 25)
            .Select(_ => new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate())
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRoundId
        };

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(25);
        result.Should().BeInAscendingOrder(x => x.Code);
    }
}
