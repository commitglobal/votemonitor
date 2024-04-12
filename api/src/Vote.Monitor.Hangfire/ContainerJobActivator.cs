using Hangfire;

namespace Vote.Monitor.Hangfire;

public class ContainerJobActivator : JobActivator
{
    private IServiceProvider _serviceProvider;

    public ContainerJobActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override object ActivateJob(Type type)
    {
        return _serviceProvider.GetService(type)!;
    }
}
