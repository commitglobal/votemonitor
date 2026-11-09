using Feature.IncidentReports.Attachments.InitiateUpload;
using Feature.IncidentReports.Attachments.Specifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Attachments.UnitTests.Endpoints;

public class InitiateUploadEndpointTests
{
    private readonly IRepository<IncidentReportAttachment> _repository;
    private readonly IReadRepository<IncidentReport> _incidentReportRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly Endpoint _endpoint;

    public InitiateUploadEndpointTests()
    {
        _repository = Substitute.For<IRepository<IncidentReportAttachment>>();
        _incidentReportRepository = Substitute.For<IReadRepository<IncidentReport>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<Endpoint>(_repository, _incidentReportRepository, _fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnConflict_AndNotCreateAttachment_WhenIncidentReportIsCompleted()
    {
        // Arrange
        _incidentReportRepository
            .AnyAsync(Arg.Any<CompletedIncidentReportSpecification>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            NumberOfUploadParts = 1
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<Conflict>();
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _fileStorageService.DidNotReceiveWithAnyArgs()
            .CreateMultipartUploadAsync(default!, default!, default!, default, default);
    }
}
