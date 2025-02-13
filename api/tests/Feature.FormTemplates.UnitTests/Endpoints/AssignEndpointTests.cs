using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class AssignEndpointTests
{
    private readonly TestContext _context;
    private readonly IRepository<ElectionRoundFormTemplate> _repository;
    private readonly AssignTemplates.Endpoint _endpoint;

    public AssignEndpointTests()
    {
        _context = TestContext.Fake();
        _repository = Substitute.For<IRepository<ElectionRoundFormTemplate>>();

        //  injectam DbContextul si repository-ul in endpoint
        _endpoint = new AssignTemplates.Endpoint(_context, _repository);
    }
    
    [Fact]
    public async Task ShouldReturnNoContent_WhenAssigmentsAreUpdatedCorrectly()
    {
        // Arrange 
        var electionRoundId = Guid.NewGuid();
        var existingTemplateId = Guid.NewGuid();
        var newTemplateId = Guid.NewGuid();
        
        // adaugam o runda de alegeri in db
        var electionRound = new ElectionRoundAggregateFaker();
        await _context.ElectionRounds.AddAsync(electionRound);
        await _context.SaveChangesAsync();
        
        // adaugam o assignare existenta
        var existingAssignment = ElectionRoundFormTemplate.Create(electionRoundId, existingTemplateId);
        await _context.ElectionRoundFormTemplates.AddAsync(existingAssignment);
        await _context.SaveChangesAsync();
        
        // cream request-ul pt asignare
        var request = new AssignTemplates
            .Request
            {
                ElectionRoundId = electionRoundId,
                FormTemplateIds = new List<Guid> { newTemplateId }
            };
        
        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        
        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
        
        // verificam asignarea in baza de date
        var assignmentsInDb = await _context.ElectionRoundFormTemplates
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Select(x => x.FormTemplateId)
            .ToListAsync();

        assignmentsInDb.Should().Contain(newTemplateId);
        assignmentsInDb.Should().NotContain(existingTemplateId);
    }
    
}
