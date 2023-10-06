using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;

internal class GetAllValidator : Validator<GetAllPollingStationsRequest>
{
    public GetAllValidator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor (x => x.Page).GreaterThan(0);
    }
      
}
