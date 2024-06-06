using Feature.Statistics.Options;
using Microsoft.Extensions.Options;

namespace Feature.Statistics.GetElectionsOverview;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .Must(apiKey =>
            {
                var options = Resolve<IOptions<StatisticsFeatureOptions>>();
                return options.Value.ApiKey == apiKey;
            });

        RuleFor(x => x.ElectionRoundIds)
            .NotEmpty();

        RuleForEach(x => x.ElectionRoundIds).NotEmpty();
    }
}
