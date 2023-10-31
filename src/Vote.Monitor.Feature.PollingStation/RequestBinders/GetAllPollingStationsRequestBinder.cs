using FastEndpoints;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
using Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;

namespace Vote.Monitor.Feature.PollingStation.RequestBinders;

internal class GetAllPollingStationsRequestBinder : RequestBinder<GetAllPollingStationsRequest>
{
    public async override ValueTask<GetAllPollingStationsRequest> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var request = await base.BindAsync(ctx, ct);

        // Special case to handle the Dictionary
        request.Filter = ctx.HttpContext.Request.Query
             .Where(x => x.Key != nameof(request.Page) && x.Key != nameof(request.PageSize))
             .ToDictionary(x => x.Key, x => x.Value.First()!);

        return request;
    }
}

internal class TagValuesRequestBinder : RequestBinder<TagValuesRequest>
{
    public async override ValueTask<TagValuesRequest> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var request = await base.BindAsync(ctx, ct);

        // Special case to handle the Dictionary
        request.Filter = ctx.HttpContext.Request.Query
             .Where(x => x.Key != nameof(request.SelectTag))
             .ToDictionary(x => x.Key, x => x.Value.First()!);

        return request;
    }
}
