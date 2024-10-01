namespace Feature.Citizen.Guides.List;

public record Response
{
    public required List<CitizenGuideModel> Guides { get; set; }
}
