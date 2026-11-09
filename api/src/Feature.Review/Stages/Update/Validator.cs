namespace Feature.Review.Stages.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(256);
        RuleFor(x => x.Status).NotNull();
        RuleFor(x => x.Color)
            .Must(color => color is null || ReviewStageAppearance.Colors.Contains(color))
            .WithMessage("Pick one of the preset colours");
        RuleFor(x => x.Icon)
            .Must(icon => icon is null || ReviewStageAppearance.Icons.Contains(icon))
            .WithMessage("Pick one of the preset icons");
    }
}
