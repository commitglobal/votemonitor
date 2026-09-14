using Feature.Form.Submission.Comments.Create;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionCommentAggregate;

namespace Feature.Form.Submission.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IReadRepository<FormSubmission> _formSubmissionRepository;
    private readonly IRepository<FormSubmissionComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _formSubmissionRepository = Substitute.For<IReadRepository<FormSubmission>>();
        _repository = Substitute.For<IRepository<FormSubmissionComment>>();
        _endpoint = Factory.Create<Endpoint>(_formSubmissionRepository, _repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var text = "an answer comment";

        _formSubmissionRepository.AnyAsync(Arg.Any<ISpecification<FormSubmission>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            SubmissionId = submissionId,
            QuestionId = questionId,
            Text = text
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .AddAsync(Arg.Is<FormSubmissionComment>(x => x.Text == text
                                                         && x.ElectionRoundId == electionRoundId
                                                         && x.SubmissionId == submissionId
                                                         && x.QuestionId == questionId));

        var model = result.Result.As<Ok<FormSubmissionCommentModel>>();
        model.Value!.Text.Should().Be(text);
        model.Value.SubmissionId.Should().Be(submissionId);
        model.Value.QuestionId.Should().Be(questionId);
    }

    [Fact]
    public async Task ShouldAddSubmissionLevelComment_WhenQuestionIdIsNull()
    {
        _formSubmissionRepository.AnyAsync(Arg.Any<ISpecification<FormSubmission>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            QuestionId = null,
            Text = "a submission comment"
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .AddAsync(Arg.Is<FormSubmissionComment>(x => x.QuestionId == null && x.Text == request.Text));

        var model = result.Result.As<Ok<FormSubmissionCommentModel>>();
        model.Value!.QuestionId.Should().BeNull();
        model.Value.Text.Should().Be(request.Text);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenSubmissionNotFromSameNgo()
    {
        _formSubmissionRepository.AnyAsync(Arg.Any<ISpecification<FormSubmission>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            Text = "an answer comment"
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<FormSubmissionComment>());
    }
}
