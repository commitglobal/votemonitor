namespace Vote.Monitor.Domain.Seeders;

public class PlatformAdminSeederSettings
{
    public PlatformAdminSeed[] PlatformAdmins { get; set; } = [];

    public class PlatformAdminSeed
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
