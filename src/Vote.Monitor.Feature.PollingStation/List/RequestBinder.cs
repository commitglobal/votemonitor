namespace Vote.Monitor.Feature.PollingStation.List;

public class RequestBinder : RequestBinder<Request>
{
    public override async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var request = await base.BindAsync(ctx, ct);

        // Special case to handle the Dictionary
        request.Filter = ctx.HttpContext.Request.Query
             .Where(x => x.Key != nameof(request.PageNumber) && x.Key != nameof(request.PageSize))
             .ToDictionary(x => x.Key, x => x.Value.First()!);

        return request;
    }
}
