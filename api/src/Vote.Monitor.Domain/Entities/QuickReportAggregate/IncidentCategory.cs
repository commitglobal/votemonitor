using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.QuickReportAggregate;

public sealed class IncidentCategory : SmartEnum<IncidentCategory, string>
{
    public static readonly IncidentCategory PhysicalViolenceIntimidationPressure =
        new(nameof(PhysicalViolenceIntimidationPressure), nameof(PhysicalViolenceIntimidationPressure));

    public static readonly IncidentCategory CampaigningAtPollingStation =
        new(nameof(CampaigningAtPollingStation), nameof(CampaigningAtPollingStation));

    public static readonly IncidentCategory RestrictionOfObserversRights =
        new(nameof(RestrictionOfObserversRights), nameof(RestrictionOfObserversRights));

    public static readonly IncidentCategory UnauthorizedPersonsAtPollingStation =
        new(nameof(UnauthorizedPersonsAtPollingStation), nameof(UnauthorizedPersonsAtPollingStation));

    public static readonly IncidentCategory ViolationDuringVoterVerificationProcess =
        new(nameof(ViolationDuringVoterVerificationProcess), nameof(ViolationDuringVoterVerificationProcess));

    public static readonly IncidentCategory VotingWithImproperDocumentation =
        new(nameof(VotingWithImproperDocumentation), nameof(VotingWithImproperDocumentation));

    public static readonly IncidentCategory IllegalRestrictionOfVotersRightToVote =
        new(nameof(IllegalRestrictionOfVotersRightToVote), nameof(IllegalRestrictionOfVotersRightToVote));

    public static readonly IncidentCategory DamagingOrSeizingElectionMaterials =
        new(nameof(DamagingOrSeizingElectionMaterials), nameof(DamagingOrSeizingElectionMaterials));

    public static readonly IncidentCategory ImproperFilingOrHandlingOfElectionDocumentation =
        new(nameof(ImproperFilingOrHandlingOfElectionDocumentation),
            nameof(ImproperFilingOrHandlingOfElectionDocumentation));

    public static readonly IncidentCategory BallotStuffing = new(nameof(BallotStuffing), nameof(BallotStuffing));

    public static readonly IncidentCategory ViolationsRelatedToControlPaper =
        new(nameof(ViolationsRelatedToControlPaper), nameof(ViolationsRelatedToControlPaper));

    public static readonly IncidentCategory NotCheckingVoterIdentificationSafeguardMeasures =
        new(nameof(NotCheckingVoterIdentificationSafeguardMeasures), nameof(NotCheckingVoterIdentificationSafeguardMeasures));

    public static readonly IncidentCategory VotingWithoutVoterIdentificationSafeguardMeasures =
        new(nameof(VotingWithoutVoterIdentificationSafeguardMeasures), nameof(VotingWithoutVoterIdentificationSafeguardMeasures));

    public static readonly IncidentCategory BreachOfSecrecyOfVote =
        new(nameof(BreachOfSecrecyOfVote), nameof(BreachOfSecrecyOfVote));

    public static readonly IncidentCategory ViolationsRelatedToMobileBallotBox =
        new(nameof(ViolationsRelatedToMobileBallotBox), nameof(ViolationsRelatedToMobileBallotBox));

    public static readonly IncidentCategory NumberOfBallotsExceedsNumberOfVoters =
        new(nameof(NumberOfBallotsExceedsNumberOfVoters), nameof(NumberOfBallotsExceedsNumberOfVoters));

    public static readonly IncidentCategory ImproperInvalidationOrValidationOfBallots =
        new(nameof(ImproperInvalidationOrValidationOfBallots), nameof(ImproperInvalidationOrValidationOfBallots));

    public static readonly IncidentCategory FalsificationOrImproperCorrectionOfFinalProtocol =
        new(nameof(FalsificationOrImproperCorrectionOfFinalProtocol),
            nameof(FalsificationOrImproperCorrectionOfFinalProtocol));

    public static readonly IncidentCategory RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy =
        new(nameof(RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy),
            nameof(RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy));

    public static readonly IncidentCategory ImproperFillingInOfFinalProtocol =
        new(nameof(ImproperFillingInOfFinalProtocol), nameof(ImproperFillingInOfFinalProtocol));

    public static readonly IncidentCategory ViolationOfSealingProceduresOfElectionMaterials =
        new(nameof(ViolationOfSealingProceduresOfElectionMaterials),
            nameof(ViolationOfSealingProceduresOfElectionMaterials));

    public static readonly IncidentCategory ViolationsRelatedToVoterLists =
        new(nameof(ViolationsRelatedToVoterLists), nameof(ViolationsRelatedToVoterLists));

    public static readonly IncidentCategory Other = new(nameof(Other), nameof(Other));


    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="IncidentCategory" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out IncidentCategory result)
    {
        return TryFromValue(value, out result);
    }

    private IncidentCategory(string name, string value) : base(name, value)
    {
    }
}