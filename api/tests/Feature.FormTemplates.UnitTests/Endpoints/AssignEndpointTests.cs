using Feature.FormTemplates.AssignTemplates;
using Feature.FormTemplates.Specifications;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.UnitTests.Endpoints;

public class AssignEndpointTests
{
    private readonly IReadRepository<ElectionRound> _electionRoundRepository;
    private readonly IReadRepository<FormTemplateAggregate> _formTemplateRepository;
    private readonly IRepository<ElectionRoundFormTemplate> _electionRoundFormTemplateRepository;
    private readonly AssignTemplates.Endpoint _endpoint;

    public AssignEndpointTests()
    {
        _electionRoundRepository = Substitute.For<IReadRepository<ElectionRound>>();
        _formTemplateRepository = Substitute.For<IReadRepository<FormTemplateAggregate>>();
        _electionRoundFormTemplateRepository = Substitute.For<IRepository<ElectionRoundFormTemplate>>();
        _endpoint = Factory.Create<Endpoint>(
            _electionRoundRepository,
            _formTemplateRepository,
            _electionRoundFormTemplateRepository);
    }

    [Fact]
    public async Task ShouldUnassignFormTemplates_WhenRequestDoesNotContainAssignedTemplates()
    {
        // Arrange
        _electionRoundRepository.AnyAsync(Arg.Any<GetElectionRoundByIdSpecification>()).Returns(true);
        _formTemplateRepository.CountAsync(Arg.Any<ListFormTemplatesByIds>()).Returns(1);

        var electionRoundId = Guid.NewGuid();
        var templateIdToKeep = Guid.NewGuid();
        var templateIdToDelete = Guid.NewGuid();

        _electionRoundFormTemplateRepository
            .ListAsync(Arg.Any<ListElectionRoundFormTemplateSpecification>())
            .Returns(new List<ElectionRoundFormTemplate>
            {
                new ElectionRoundFormTemplateAggregateFaker(electionRoundId, templateIdToKeep),
                new ElectionRoundFormTemplateAggregateFaker(electionRoundId, templateIdToDelete),
            });


        // Act
        var request = new Request { ElectionRoundId = electionRoundId, FormTemplateIds = [templateIdToKeep] };

        var response = await _endpoint.ExecuteAsync(request, default);

        // Assert
        response.Should().BeOfType<Results<NoContent, NotFound>>()!
            .Which!
            .Result.Should().BeOfType<NoContent>();

        await _electionRoundFormTemplateRepository.Received(1)
            .DeleteRangeAsync(Arg.Is<IEnumerable<ElectionRoundFormTemplate>>(
                x => x.Count() == 1
                     && x.Any(y => y.FormTemplateId == templateIdToDelete)), Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task ShouldAssignFormTemplates_WhenRequestContainUnassignedTemplates()
    {
        // Arrange
        _electionRoundRepository.AnyAsync(Arg.Any<GetElectionRoundByIdSpecification>()).Returns(true);
        _formTemplateRepository.CountAsync(Arg.Any<ListFormTemplatesByIds>()).Returns(2);
        _electionRoundFormTemplateRepository
            .ListAsync(Arg.Any<ListElectionRoundFormTemplateSpecification>()).Returns([]);

        var formTemplateId1 = Guid.NewGuid();
        var formTemplateId2 = Guid.NewGuid();


        // Act
        var request = new Request { FormTemplateIds = [formTemplateId1, formTemplateId2] };

        var response = await _endpoint.ExecuteAsync(request, default);

        // Assert
        response.Should().BeOfType<Results<NoContent, NotFound>>()!
            .Which!
            .Result.Should().BeOfType<NoContent>();

        await _electionRoundFormTemplateRepository.Received(1)
            .AddRangeAsync(Arg.Is<IEnumerable<ElectionRoundFormTemplate>>(x => x.Count() == 2
                                                                               && x.Any(y =>
                                                                                   y.FormTemplateId == formTemplateId1)
                                                                               && x.Any(y =>
                                                                                   y.FormTemplateId ==
                                                                                   formTemplateId2)),
                Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundDoesNotExist()
    {
        // Arange
        _electionRoundRepository.AnyAsync(Arg.Any<GetElectionRoundByIdSpecification>()).Returns(false);
        
        // Act
        var response = await _endpoint.ExecuteAsync(new Request(), default);

        // assert
        response.Should().BeOfType<Results<NoContent, NotFound>>()!
            .Which!
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormTemplatesNotFound()
    {
        // Arrange
        _electionRoundRepository.AnyAsync(Arg.Any<GetElectionRoundByIdSpecification>()).Returns(true);
        _formTemplateRepository.CountAsync(Arg.Any<ListFormTemplatesByIds>()).Returns(1);

        var formTemplateId1 = Guid.NewGuid(); 
        var formTemplateId2 = Guid.NewGuid(); 
            
        // Act
        var request = new Request { FormTemplateIds = [formTemplateId1, formTemplateId2] };

        var response = await _endpoint.ExecuteAsync(request, default);

        // Assert
        response.Should().BeOfType<Results<NoContent, NotFound>>()!
            .Which!
            .Result.Should().BeOfType<NotFound>();
    }
}
