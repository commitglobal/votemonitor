namespace Feature.Dashboards.Update;

public class Validator : SaveDashboardRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.DashboardId).NotEmpty();
    }
}
