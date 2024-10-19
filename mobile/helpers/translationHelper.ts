import i18n from "../common/config/i18n";
import { IncidentCategory } from "../services/api/quick-report/post-quick-report.api";

export const localizeIncidentCategory = (incidentCategory: IncidentCategory): string => {
  if (incidentCategory === IncidentCategory.PhysicalViolenceIntimidationPressure)
    return i18n.t("form.incident_category.options.PhysicalViolenceIntimidationPressure", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.CampaigningAtPollingStation)
    return i18n.t("form.incident_category.options.CampaigningAtPollingStation", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.RestrictionOfObserversRights)
    return i18n.t("form.incident_category.options.RestrictionOfObserversRights", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.UnauthorizedPersonsAtPollingStation)
    return i18n.t("form.incident_category.options.UnauthorizedPersonsAtPollingStation", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.ViolationDuringVoterVerificationProcess)
    return i18n.t("form.incident_category.options.ViolationDuringVoterVerificationProcess", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.VotingWithImproperDocumentation)
    return i18n.t("form.incident_category.options.VotingWithImproperDocumentation", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.IllegalRestrictionOfVotersRightToVote)
    return i18n.t("form.incident_category.options.IllegalRestrictionOfVotersRightToVote", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.DamagingOrSeizingElectionMaterials)
    return i18n.t("form.incident_category.options.DamagingOrSeizingElectionMaterials", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.ImproperFilingOrHandlingOfElectionDocumentation)
    return i18n.t(
      "form.incident_category.options.ImproperFilingOrHandlingOfElectionDocumentation",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.BallotStuffing)
    return i18n.t("form.incident_category.options.BallotStuffing", { ns: "report_new_issue" });
  if (incidentCategory === IncidentCategory.ViolationsRelatedToControlPaper)
    return i18n.t("form.incident_category.options.ViolationsRelatedToControlPaper", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.NotCheckingVoterIdentificationSafeguardMeasures)
    return i18n.t(
      "form.incident_category.options.NotCheckingVoterIdentificationSafeguardMeasures",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures)
    return i18n.t(
      "form.incident_category.options.VotingWithoutVoterIdentificationSafeguardMeasures",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.BreachOfSecrecyOfVote)
    return i18n.t("form.incident_category.options.BreachOfSecrecyOfVote", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.ViolationsRelatedToMobileBallotBox)
    return i18n.t("form.incident_category.options.ViolationsRelatedToMobileBallotBox", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.NumberOfBallotsExceedsNumberOfVoters)
    return i18n.t("form.incident_category.options.NumberOfBallotsExceedsNumberOfVoters", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.ImproperInvalidationOrValidationOfBallots)
    return i18n.t("form.incident_category.options.ImproperInvalidationOrValidationOfBallots", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol)
    return i18n.t(
      "form.incident_category.options.FalsificationOrImproperCorrectionOfFinalProtocol",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy)
    return i18n.t(
      "form.incident_category.options.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.ImproperFillingInOfFinalProtocol)
    return i18n.t("form.incident_category.options.ImproperFillingInOfFinalProtocol", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.ViolationOfSealingProceduresOfElectionMaterials)
    return i18n.t(
      "form.incident_category.options.ViolationOfSealingProceduresOfElectionMaterials",
      { ns: "report_new_issue" },
    );
  if (incidentCategory === IncidentCategory.ViolationsRelatedToVoterLists)
    return i18n.t("form.incident_category.options.ViolationsRelatedToVoterLists", {
      ns: "report_new_issue",
    });
  if (incidentCategory === IncidentCategory.Other)
    return i18n.t("form.incident_category.options.Other", { ns: "report_new_issue" });

  return incidentCategory;
};
