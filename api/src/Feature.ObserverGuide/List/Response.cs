using Feature.ObserverGuide.Model;

namespace Feature.ObserverGuide.List;

public record Response
{
    public required List<ObserverGuideModel> Guides { get; set; }
}
