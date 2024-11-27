namespace Vote.Monitor.Api.IntegrationTests.Consts;

public enum ScenarioNgo
{
    Alfa,
    Beta,
    Delta
}

public class ScenarioNgos
{
    public static AlfaDetails Alfa => new();
    public static BetaDetails Beta => new BetaDetails();
    public static DeltaDetails Delta => new DeltaDetails();
    
    public class AlfaDetails
    {
        public static implicit operator ScenarioNgo(AlfaDetails _) => ScenarioNgo.Alfa;
        public readonly string Anya = "anya@alfa.com";
        public readonly string Ben = "ben@alfa.com";
        public readonly string Cody = "cody@alfa.com";
    }

    public class BetaDetails
    {
        public static implicit operator ScenarioNgo(BetaDetails _) => ScenarioNgo.Beta;
        public readonly string Dana = "dana@beta.com";
        public readonly string Ivy = "ivy@beta.com";
        public readonly string Finn = "finn@beta.com";
    }

    public class DeltaDetails
    {
        public static implicit operator ScenarioNgo(DeltaDetails _) => ScenarioNgo.Delta;
        public readonly string Gia = "gia@delta.com";
        public readonly string Mason = "mason@delta.com";
        public readonly string Omar = "omar@delta.com";
    }
}
