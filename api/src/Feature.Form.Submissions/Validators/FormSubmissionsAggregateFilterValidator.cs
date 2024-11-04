using Feature.Form.Submissions.Requests;

namespace Feature.Form.Submissions.Validators;

public class FormSubmissionsAggregateFilterValidator : Validator<FormSubmissionsAggregateFilter>
{
    public FormSubmissionsAggregateFilterValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
    }
}
