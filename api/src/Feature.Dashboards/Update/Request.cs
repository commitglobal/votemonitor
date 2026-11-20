namespace Feature.Dashboards.Update;

public class Request : SaveDashboardRequest
{
    public Guid DashboardId { get; set; }

    /// <summary>
    /// The version the change was made from (concurrent edits contract): a newer one on the server is a 409 and nothing
    /// is saved. Left out: saved without a check.
    /// </summary>
    public uint? Version { get; set; }
}
