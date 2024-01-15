using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Core.Preprocessors;

public class CurrentUserInjector : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext ctx, CancellationToken ct)
    {
        var currentUserInitializer = ctx.HttpContext.Resolve<ICurrentUserInitializer>();
        currentUserInitializer.SetCurrentUser(ctx.HttpContext.User);

        return Task.CompletedTask;
    }
}
