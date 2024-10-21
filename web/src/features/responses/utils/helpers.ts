import {
  CitizenReportFollowUpStatus,
  FormSubmissionFollowUpStatus,
  IncidentReportFollowUpStatus,
  QuestionsAnswered,
  QuickReportFollowUpStatus,
} from '@/common/types';
import { IncidentCategory, QuickReportLocationType } from '../models/quick-report';
import { IncidentReportLocationType } from '../models/incident-report';
import i18n from '@/i18n';

export function mapQuickReportLocationType(locationType: QuickReportLocationType): string {
  if (locationType === QuickReportLocationType.NotRelatedToAPollingStation)
    return i18n.t('quickReports.locationType.NotRelatedToAPollingStation');
  if (locationType === QuickReportLocationType.OtherPollingStation)
    return i18n.t('quickReports.locationType.OtherPollingStation');
  if (locationType === QuickReportLocationType.VisitedPollingStation)
    return i18n.t('quickReports.locationType.VisitedPollingStation');

  return locationType;
}

export function mapFormSubmissionFollowUpStatus(followUpStatus: FormSubmissionFollowUpStatus): string {
  if (followUpStatus === FormSubmissionFollowUpStatus.NotApplicable)
    return i18n.t('formSubmissions.followUpStatus.NotApplicable');
  if (followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp)
    return i18n.t('formSubmissions.followUpStatus.NeedsFollowUp');
  if (followUpStatus === FormSubmissionFollowUpStatus.Resolved) return i18n.t('formSubmissions.followUpStatus.Resolved');

  return followUpStatus;
}

export function mapQuickReportFollowUpStatus(followUpStatus: QuickReportFollowUpStatus): string {
  if (followUpStatus === QuickReportFollowUpStatus.NotApplicable)
    return i18n.t('quickReports.followUpStatus.NotApplicable');
  if (followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp)
    return i18n.t('quickReports.followUpStatus.NeedsFollowUp');
  if (followUpStatus === QuickReportFollowUpStatus.Resolved) return i18n.t('quickReports.followUpStatus.Resolved');

  return followUpStatus;
}

export function mapCitizenReportFollowUpStatus(followUpStatus: CitizenReportFollowUpStatus): string {
  if (followUpStatus === CitizenReportFollowUpStatus.NotApplicable)
    return i18n.t('citizenReports.followUpStatus.NotApplicable');
  if (followUpStatus === CitizenReportFollowUpStatus.NeedsFollowUp)
    return i18n.t('citizenReports.followUpStatus.NeedsFollowUp');
  if (followUpStatus === CitizenReportFollowUpStatus.Resolved) return i18n.t('citizenReports.followUpStatus.Resolved');

  return followUpStatus;
}

export function mapIncidentReportFollowUpStatus(followUpStatus: IncidentReportFollowUpStatus): string {
  if (followUpStatus === IncidentReportFollowUpStatus.NotApplicable)
    return i18n.t('incidentReports.followUpStatus.NotApplicable');
  if (followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp)
    return i18n.t('incidentReports.followUpStatus.NeedsFollowUp');
  if (followUpStatus === IncidentReportFollowUpStatus.Resolved)
    return i18n.t('incidentReports.followUpStatus.Resolved');

  return followUpStatus;
}

export function mapIncidentReportLocationType(locationType: IncidentReportLocationType): string {
  if (locationType === IncidentReportLocationType.PollingStation)
    return i18n.t('incidentReports.locationType.PollingStation');
  if (locationType === IncidentReportLocationType.OtherLocation)
    return i18n.t('incidentReports.locationType.OtherLocation');

  return locationType;
}

export function mapQuestionsAnswered(questionsAnswered: QuestionsAnswered): string {
  if (questionsAnswered === QuestionsAnswered.None) return i18n.t('enums.questionsAnswered.None');
  if (questionsAnswered === QuestionsAnswered.Some) return i18n.t('enums.questionsAnswered.Some');
  if (questionsAnswered === QuestionsAnswered.All) return i18n.t('enums.questionsAnswered.All');

  return questionsAnswered;
}

export function mapIncidentCategory(incidentCategory: IncidentCategory): string {
  if (incidentCategory === IncidentCategory.PhysicalViolenceIntimidationPressure)
    return i18n.t('quickReports.incidentCategory.PhysicalViolenceIntimidationPressure');
  if (incidentCategory === IncidentCategory.CampaigningAtPollingStation)
    return i18n.t('quickReports.incidentCategory.CampaigningAtPollingStation');
  if (incidentCategory === IncidentCategory.RestrictionOfObserversRights)
    return i18n.t('quickReports.incidentCategory.RestrictionOfObserversRights');
  if (incidentCategory === IncidentCategory.UnauthorizedPersonsAtPollingStation)
    return i18n.t('quickReports.incidentCategory.UnauthorizedPersonsAtPollingStation');
  if (incidentCategory === IncidentCategory.ViolationDuringVoterVerificationProcess)
    return i18n.t('quickReports.incidentCategory.ViolationDuringVoterVerificationProcess');
  if (incidentCategory === IncidentCategory.VotingWithImproperDocumentation)
    return i18n.t('quickReports.incidentCategory.VotingWithImproperDocumentation');
  if (incidentCategory === IncidentCategory.IllegalRestrictionOfVotersRightToVote)
    return i18n.t('quickReports.incidentCategory.IllegalRestrictionOfVotersRightToVote');
  if (incidentCategory === IncidentCategory.DamagingOrSeizingElectionMaterials)
    return i18n.t('quickReports.incidentCategory.DamagingOrSeizingElectionMaterials');
  if (incidentCategory === IncidentCategory.ImproperFilingOrHandlingOfElectionDocumentation)
    return i18n.t('quickReports.incidentCategory.ImproperFilingOrHandlingOfElectionDocumentation');
  if (incidentCategory === IncidentCategory.BallotStuffing)
    return i18n.t('quickReports.incidentCategory.BallotStuffing');
  if (incidentCategory === IncidentCategory.ViolationsRelatedToControlPaper)
    return i18n.t('quickReports.incidentCategory.ViolationsRelatedToControlPaper');
  if (incidentCategory === IncidentCategory.NotCheckingVoterIdentificationSafeguardMeasures)
    return i18n.t('quickReports.incidentCategory.NotCheckingVoterIdentificationSafeguardMeasures');
  if (incidentCategory === IncidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures)
    return i18n.t('quickReports.incidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures');
  if (incidentCategory === IncidentCategory.BreachOfSecrecyOfVote)
    return i18n.t('quickReports.incidentCategory.BreachOfSecrecyOfVote');
  if (incidentCategory === IncidentCategory.ViolationsRelatedToMobileBallotBox)
    return i18n.t('quickReports.incidentCategory.ViolationsRelatedToMobileBallotBox');
  if (incidentCategory === IncidentCategory.NumberOfBallotsExceedsNumberOfVoters)
    return i18n.t('quickReports.incidentCategory.NumberOfBallotsExceedsNumberOfVoters');
  if (incidentCategory === IncidentCategory.ImproperInvalidationOrValidationOfBallots)
    return i18n.t('quickReports.incidentCategory.ImproperInvalidationOrValidationOfBallots');
  if (incidentCategory === IncidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol)
    return i18n.t('quickReports.incidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol');
  if (incidentCategory === IncidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy)
    return i18n.t('quickReports.incidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy');
  if (incidentCategory === IncidentCategory.ImproperFillingInOfFinalProtocol)
    return i18n.t('quickReports.incidentCategory.ImproperFillingInOfFinalProtocol');
  if (incidentCategory === IncidentCategory.ViolationOfSealingProceduresOfElectionMaterials)
    return i18n.t('quickReports.incidentCategory.ViolationOfSealingProceduresOfElectionMaterials');
  if (incidentCategory === IncidentCategory.ViolationsRelatedToVoterLists)
    return i18n.t('quickReports.incidentCategory.ViolationsRelatedToVoterLists');
  if (incidentCategory === IncidentCategory.Other) return i18n.t('quickReports.incidentCategory.Other');

  return incidentCategory;
}
