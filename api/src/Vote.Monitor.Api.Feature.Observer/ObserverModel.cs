namespace Vote.Monitor.Api.Feature.Observer;

public record ObserverModel
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public bool IsAccountVerified { get; init; }


    public string? PhoneNumber { get; init; }
    public UserStatus Status { get; init; }
    
    public MonitoredElectionsDetails[] MonitoredElections { get; init; } = [];

    public record MonitoredElectionsDetails
    {
        public string ElectionTitle { get; init; }
        public string ElectionEnglishTitle { get; init; }
        public string ElectionDate { get; init; }
        public string NgoName { get; init; }
        
    }
}
