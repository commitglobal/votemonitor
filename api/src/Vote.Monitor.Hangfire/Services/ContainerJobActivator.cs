using Hangfire;

namespace Vote.Monitor.Hangfire.Services;

public class ContainerJobActivator(IServiceProvider serviceProvider) : JobActivator
{
    public override object ActivateJob(Type type)
    {
        return serviceProvider.CreateScope().ServiceProvider.GetRequiredService(type);
    }
}
