namespace Feature.Forms.UnitTests.ValidatorTests;

public class UpdateDisplayOrderRequestValidatorTests
{
    private readonly UpdateDisplayOrder.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        var request = new UpdateDisplayOrder.Request { ElectionRoundId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        var request = new UpdateDisplayOrder.Request { NgoId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Forms_Empty()
    {
        var request = new UpdateDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms = []
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Forms);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormIds_NotUnique()
    {
        var formId = Guid.NewGuid();
        var request = new UpdateDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = formId, DisplayOrder = 1 },
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = formId, DisplayOrder = 2 }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Forms);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormId_Empty()
    {
        var request = new UpdateDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = Guid.Empty, DisplayOrder = 1 }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("Forms[0].FormId");
    }

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_Negative()
    {
        var request = new UpdateDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = Guid.NewGuid(), DisplayOrder = -1 }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("Forms[0].DisplayOrder");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        var request = new UpdateDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = Guid.NewGuid(), DisplayOrder = 0 },
                new UpdateDisplayOrder.FormDisplayOrderModel { FormId = Guid.NewGuid(), DisplayOrder = 1 }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
