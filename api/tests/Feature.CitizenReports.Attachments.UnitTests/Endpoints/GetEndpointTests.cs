using FastEndpoints;
using Feature.CitizenReports.Attachments.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.CitizenReports.Attachments.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<CitizenReportAttachment> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<CitizenReportAttachment>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository, fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var citizenReportAttachment = new CitizenReportAttachmentFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .Returns(citizenReportAttachment);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = citizenReportAttachment.ElectionRoundId,
            CitizenReportId = citizenReportAttachment.CitizenReportId,
            Id = citizenReportAttachment.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<CitizenReportsAttachmentModel>>();
        model.Value!.FileName.Should().Be(citizenReportAttachment.FileName);
        model.Value.Id.Should().Be(citizenReportAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAttachmentDoesNotExist()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CitizenReportsAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}