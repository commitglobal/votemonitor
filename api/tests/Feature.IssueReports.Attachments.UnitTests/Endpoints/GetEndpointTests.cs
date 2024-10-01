using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute.ReturnsExtensions;

namespace Feature.IssueReports.Attachments.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<IssueReportAttachment> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IssueReportAttachment>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository, fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var issueReportAttachment = new IssueReportAttachmentFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .Returns(issueReportAttachment);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = issueReportAttachment.ElectionRoundId,
            IssueReportId = issueReportAttachment.IssueReportId,
            Id = issueReportAttachment.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<IssueReportAttachmentModel>>();
        model.Value!.FileName.Should().Be(issueReportAttachment.FileName);
        model.Value.Id.Should().Be(issueReportAttachment.Id);
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
            IssueReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<IssueReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}