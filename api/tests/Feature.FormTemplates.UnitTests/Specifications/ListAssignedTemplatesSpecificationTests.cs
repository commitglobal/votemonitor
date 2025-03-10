using Feature.FormTemplates.ListAssignedTemplates;
using Feature.FormTemplates.Specifications;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.FormTemplates.UnitTests.Specifications;

public class ListAssignedTemplatesSpecificationTests
{
    private readonly FormStatus DefaultStatus = FormStatus.Drafted;

    [Fact]
    public void ListAssignedTemplatesSpecification_Should_Filter_By_ElectionRoundId_And_Return_Correct_FormTemplates()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();
        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PageNumber = 1,
            PageSize = 10
        };
        
        var matchingTemplate = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var secondMatchingTemaplate = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var nonMatchingTemplate = new ElectionRoundFormTemplateAggregateFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var testCollection = new List<ElectionRoundFormTemplate>
        {
            matchingTemplate,
            secondMatchingTemaplate,
            nonMatchingTemplate
        }.AsQueryable();

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();
        
        // Assert
        var expectedIds = new List<Guid> {matchingTemplate.FormTemplate.Id, secondMatchingTemaplate.FormTemplate.Id };
        
        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().BeEquivalentTo(expectedIds);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ListAssignedTemplatesSpecification_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();
        var template1 = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var template2 = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();


        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate())
            .Union(new[] { template1, template2 })
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            SortColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(x => x.LastModifiedOn);
    }
    
    [Theory]
    [MemberData(nameof(NameSortingTestCases))]
    public void ListAssignedFormTemplatesSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();


        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate())
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            SortColumnName = columnName,
            SortOrder = sortOrder,
            PageSize = 100,
            PageNumber = 1
        };

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(100);
        
        var expectedCodes = (sortOrder == SortOrder.Desc)
            ? testCollection.OrderByDescending(x => x.FormTemplate.Code).Select(x => x.FormTemplate.Code).ToList()
            : testCollection.OrderBy(x => x.FormTemplate.Code).Select(x => x.FormTemplate.Code).ToList(); // Default is ascending
        
        var actualCodes = result.Select(x => x.Code).ToList();
        
        actualCodes.Should().BeEquivalentTo(expectedCodes);
    }
    
    [Fact]
    public void ListAssignedTemplatesSpecification_PaginatesCorrectly()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();
        var template1 = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();
        var template2 = new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate();


        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ElectionRoundFormTemplateAggregateFaker(electionRoundId, formTemplateId).Generate())
            .Union(new[] { template1, template2 })
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListAssignedFormTemplateSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
    }
    
    
    
    public static IEnumerable<object[]> NameSortingTestCases =>
        new List<object[]>
        {
            new object[] { "code", null },
            new object[] { "Code", null },
            new object[] { "code", SortOrder.Asc },
            new object[] { "Code", SortOrder.Asc }
        };
}
