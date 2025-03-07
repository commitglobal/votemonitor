using FastEndpoints;
using Feature.CitizenReports.Attachments.Specifications;
using FluentAssertions;
using NSubstitute;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.CitizenReports.Attachments.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<CitizenReportAttachment> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<CitizenReportAttachment>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<List.Endpoint>(_repository, fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var citizenReportAttachment = new CitizenReportAttachmentFaker().Generate();
        var anotherCitizenReportAttachment = new CitizenReportAttachmentFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListCitizenReportAttachmentsSpecification>())
            .Returns([citizenReportAttachment, anotherCitizenReportAttachment]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = citizenReportAttachment.ElectionRoundId,
            CitizenReportId = citizenReportAttachment.CitizenReportId,
            FormId = citizenReportAttachment.FormId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        var attachments = result.Attachments;

        attachments.Should().HaveCount(2);
        attachments.First().FileName.Should().Be(citizenReportAttachment.FileName);
        attachments.First().Id.Should().Be(citizenReportAttachment.Id);
        attachments.Last().FileName.Should().Be(anotherCitizenReportAttachment.FileName);
        attachments.Last().Id.Should().Be(anotherCitizenReportAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenAttachmentsDoNotExist()
    {
        // Arrange

        _repository
            .ListAsync(Arg.Any<ListCitizenReportAttachmentsSpecification>(), CancellationToken.None)
            .Returns([]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            FormId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        var attachments = result.Attachments;
        attachments.Should().BeEmpty();
    }
}