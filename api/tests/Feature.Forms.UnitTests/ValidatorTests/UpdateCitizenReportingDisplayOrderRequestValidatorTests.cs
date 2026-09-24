namespace Feature.Forms.UnitTests.ValidatorTests;

public class UpdateCitizenReportingDisplayOrderRequestValidatorTests
{
    private readonly UpdateCitizenReportingDisplayOrder.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        var request = new UpdateCitizenReportingDisplayOrder.Request { ElectionRoundId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        var request = new UpdateCitizenReportingDisplayOrder.Request { NgoId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Forms_Empty()
    {
        var request = new UpdateCitizenReportingDisplayOrder.Request
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
        var request = new UpdateCitizenReportingDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateCitizenReportingDisplayOrder.FormDisplayOrderModel { FormId = formId, DisplayOrder = 1 },
                new UpdateCitizenReportingDisplayOrder.FormDisplayOrderModel { FormId = formId, DisplayOrder = 2 }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Forms);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        var request = new UpdateCitizenReportingDisplayOrder.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Forms =
            [
                new UpdateCitizenReportingDisplayOrder.FormDisplayOrderModel
                {
                    FormId = Guid.NewGuid(),
                    DisplayOrder = 0
                },
                new UpdateCitizenReportingDisplayOrder.FormDisplayOrderModel
                {
                    FormId = Guid.NewGuid(),
                    DisplayOrder = 1
                }
            ]
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
