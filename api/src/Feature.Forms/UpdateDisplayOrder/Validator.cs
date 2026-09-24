namespace Feature.Forms.UpdateDisplayOrder;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Forms).NotEmpty();
        RuleFor(x => x.Forms)
            .Must(forms => forms.Select(f => f.FormId).Distinct().Count() == forms.Count)
            .WithMessage("Form ids must be unique.");

        RuleForEach(x => x.Forms).ChildRules(form =>
        {
            form.RuleFor(x => x.FormId).NotEmpty();
            form.RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        });
    }
}
