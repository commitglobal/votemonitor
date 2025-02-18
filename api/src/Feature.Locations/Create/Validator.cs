namespace Feature.Locations.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleForEach(x => x.Locations).SetValidator(new LocationRequestValidator());
    }

    public class LocationRequestValidator : Validator<Request.LocationRequest>
    {
        public LocationRequestValidator()
        {
            RuleFor(x => x.Level1).NotEmpty();
            RuleFor(x => x.Level2).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level3));
            RuleFor(x => x.Level3).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level4));
            RuleFor(x => x.Level4).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level5));
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0);
        }
    }
}
