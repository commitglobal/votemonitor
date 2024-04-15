using Hangfire;

namespace Vote.Monitor.Hangfire;

public class ContainerJobActivator(IServiceProvider serviceProvider) : JobActivator
{
    public override object ActivateJob(Type type)
    {
        return serviceProvider.GetRequiredService(type);
    }
}
