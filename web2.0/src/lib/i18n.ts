import {
  QuickReportFollowUpStatus,
  QuickReportIncidentCategory,
  QuickReportLocationType,
} from "@/types/quick-reports";
import i18n from "@/i18n";
import { FormSubmissionFollowUpStatus } from "@/types/forms-submission";

export function mapQuickReportLocationType(
  locationType: QuickReportLocationType
): string {
  if (locationType === QuickReportLocationType.NotRelatedToAPollingStation)
    return i18n.t("quickReports.locationType.NotRelatedToAPollingStation");
  if (locationType === QuickReportLocationType.OtherPollingStation)
    return i18n.t("quickReports.locationType.OtherPollingStation");
  if (locationType === QuickReportLocationType.VisitedPollingStation)
    return i18n.t("quickReports.locationType.VisitedPollingStation");

  return locationType;
}

export function mapQuickReportFollowUpStatus(
  followUpStatus: QuickReportFollowUpStatus
): string {
  if (followUpStatus === QuickReportFollowUpStatus.NotApplicable)
    return i18n.t("quickReports.followUpStatus.NotApplicable");
  if (followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp)
    return i18n.t("quickReports.followUpStatus.NeedsFollowUp");
  if (followUpStatus === QuickReportFollowUpStatus.Resolved)
    return i18n.t("quickReports.followUpStatus.Resolved");

  return followUpStatus;
}

export function mapQuickReportIncidentCategory(
  incidentCategory: QuickReportIncidentCategory
): string {
  if (
    incidentCategory ===
    QuickReportIncidentCategory.PhysicalViolenceIntimidationPressure
  )
    return i18n.t(
      "quickReports.incidentCategory.PhysicalViolenceIntimidationPressure"
    );
  if (
    incidentCategory === QuickReportIncidentCategory.CampaigningAtPollingStation
  )
    return i18n.t("quickReports.incidentCategory.CampaigningAtPollingStation");
  if (
    incidentCategory ===
    QuickReportIncidentCategory.RestrictionOfObserversRights
  )
    return i18n.t("quickReports.incidentCategory.RestrictionOfObserversRights");
  if (
    incidentCategory ===
    QuickReportIncidentCategory.UnauthorizedPersonsAtPollingStation
  )
    return i18n.t(
      "quickReports.incidentCategory.UnauthorizedPersonsAtPollingStation"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ViolationDuringVoterVerificationProcess
  )
    return i18n.t(
      "quickReports.incidentCategory.ViolationDuringVoterVerificationProcess"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.VotingWithImproperDocumentation
  )
    return i18n.t(
      "quickReports.incidentCategory.VotingWithImproperDocumentation"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.IllegalRestrictionOfVotersRightToVote
  )
    return i18n.t(
      "quickReports.incidentCategory.IllegalRestrictionOfVotersRightToVote"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.DamagingOrSeizingElectionMaterials
  )
    return i18n.t(
      "quickReports.incidentCategory.DamagingOrSeizingElectionMaterials"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ImproperFilingOrHandlingOfElectionDocumentation
  )
    return i18n.t(
      "quickReports.incidentCategory.ImproperFilingOrHandlingOfElectionDocumentation"
    );
  if (incidentCategory === QuickReportIncidentCategory.BallotStuffing)
    return i18n.t("quickReports.incidentCategory.BallotStuffing");
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ViolationsRelatedToControlPaper
  )
    return i18n.t(
      "quickReports.incidentCategory.ViolationsRelatedToControlPaper"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.NotCheckingVoterIdentificationSafeguardMeasures
  )
    return i18n.t(
      "quickReports.incidentCategory.NotCheckingVoterIdentificationSafeguardMeasures"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures
  )
    return i18n.t(
      "quickReports.incidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures"
    );
  if (incidentCategory === QuickReportIncidentCategory.BreachOfSecrecyOfVote)
    return i18n.t("quickReports.incidentCategory.BreachOfSecrecyOfVote");
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ViolationsRelatedToMobileBallotBox
  )
    return i18n.t(
      "quickReports.incidentCategory.ViolationsRelatedToMobileBallotBox"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.NumberOfBallotsExceedsNumberOfVoters
  )
    return i18n.t(
      "quickReports.incidentCategory.NumberOfBallotsExceedsNumberOfVoters"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ImproperInvalidationOrValidationOfBallots
  )
    return i18n.t(
      "quickReports.incidentCategory.ImproperInvalidationOrValidationOfBallots"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol
  )
    return i18n.t(
      "quickReports.incidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy
  )
    return i18n.t(
      "quickReports.incidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ImproperFillingInOfFinalProtocol
  )
    return i18n.t(
      "quickReports.incidentCategory.ImproperFillingInOfFinalProtocol"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ViolationOfSealingProceduresOfElectionMaterials
  )
    return i18n.t(
      "quickReports.incidentCategory.ViolationOfSealingProceduresOfElectionMaterials"
    );
  if (
    incidentCategory ===
    QuickReportIncidentCategory.ViolationsRelatedToVoterLists
  )
    return i18n.t(
      "quickReports.incidentCategory.ViolationsRelatedToVoterLists"
    );
  if (incidentCategory === QuickReportIncidentCategory.Other)
    return i18n.t("quickReports.incidentCategory.Other");

  return incidentCategory;
}

export function mapFormSubmissionFollowUpStatus(
  followUpStatus: FormSubmissionFollowUpStatus
): string {
  if (followUpStatus === FormSubmissionFollowUpStatus.NotApplicable)
    return i18n.t("formSubmissions.followUpStatus.NotApplicable");
  if (followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp)
    return i18n.t("formSubmissions.followUpStatus.NeedsFollowUp");
  if (followUpStatus === FormSubmissionFollowUpStatus.Resolved)
    return i18n.t("formSubmissions.followUpStatus.Resolved");

  return followUpStatus;
}
