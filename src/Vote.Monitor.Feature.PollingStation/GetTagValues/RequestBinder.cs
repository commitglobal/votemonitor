using FastEndpoints;

namespace Vote.Monitor.Feature.PollingStation.GetTagValues;

internal class RequestBinder : RequestBinder<Request>
{
    public override async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var request = await base.BindAsync(ctx, ct);

        // Special case to handle the Dictionary
        request.Filter = ctx.HttpContext.Request.Query
            .Where(x => x.Key != nameof(request.SelectTag))
            .ToDictionary(x => x.Key, x => x.Value.First()!);

        return request;
    }
}
