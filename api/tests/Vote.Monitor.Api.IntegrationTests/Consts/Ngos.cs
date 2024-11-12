namespace Vote.Monitor.Api.IntegrationTests.Consts;

public class Ngos
{
    public static AlfaDetails Alfa => new();
    public static BetaDetails Beta => new BetaDetails();
    public static DeltaDetails Delta => new DeltaDetails();


    public class AlfaDetails
    {
        public static implicit operator string(AlfaDetails _) => "Alfa NGO";
        public readonly string Anya = "anya@alfa.com";
        public readonly string Ben = "ben@alfa.com";
        public readonly string Cody = "cody@alfa.com";
    }

    public class BetaDetails
    {
        public static implicit operator string(BetaDetails _) => "Beta NGO";
        public readonly string Dana = "dana@beta.com";
        public readonly string Ivy = "ivy@beta.com";
        public readonly string Finn = "finn@beta.com";
    }

    public class DeltaDetails
    {
        public static implicit operator string(DeltaDetails _) => "Delta NGO";
        public readonly string Gia = "gia@delta.com";
        public readonly string Mason = "mason@delta.com";
        public readonly string Omar = "omar@delta.com";
    }
}
