using Feature.Form.Submission.Comments.Create;

namespace Feature.Form.Submission.Comments.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        var request = new Request { ElectionRoundId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        var request = new Request { NgoId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_SubmissionId_Empty()
    {
        var request = new Request { SubmissionId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SubmissionId);
    }

    [Fact]
    public void Validation_ShouldFail_When_QuestionId_Empty()
    {
        var request = new Request { QuestionId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Text_Empty(string text)
    {
        var request = new Request { Text = text };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validation_ShouldFail_When_Text_ExceedsLimits()
    {
        var request = new Request { Text = "a".Repeat(10_001) };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidSubmissionLevelComment()
    {
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            QuestionId = null,
            Text = "a submission comment"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidAnswerLevelComment()
    {
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            Text = "an answer comment"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
