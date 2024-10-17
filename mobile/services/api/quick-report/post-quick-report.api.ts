import API from "../../api";
import { QuickReportsAPIResponse } from "./get-quick-reports.api";

/** ========================================================================
    ================= POST quickReport ====================
    ========================================================================
    @description Upsert a Quick Report
    @param {AddQuickReportAPIPayload} payload
*/
export enum QuickReportLocationType {
  NotRelatedToAPollingStation = "NotRelatedToAPollingStation",
  OtherPollingStation = "OtherPollingStation",
  VisitedPollingStation = "VisitedPollingStation",
}

export enum IncidentCategory {
  PhysicalViolenceIntimidationPressure = 'PhysicalViolenceIntimidationPressure',
  CampaigningAtPollingStation = 'CampaigningAtPollingStation',
  RestrictionOfObserversRights = 'RestrictionOfObserversRights',
  UnauthorizedPersonsAtPollingStation = 'UnauthorizedPersonsAtPollingStation',
  ViolationDuringVoterVerificationProcess = 'ViolationDuringVoterVerificationProcess',
  VotingWithImproperDocumentation = 'VotingWithImproperDocumentation',
  IllegalRestrictionOfVotersRightToVote = 'IllegalRestrictionOfVotersRightToVote',
  DamagingOrSeizingElectionMaterials = 'DamagingOrSeizingElectionMaterials',
  ImproperFilingOrHandlingOfElectionDocumentation = 'ImproperFilingOrHandlingOfElectionDocumentation',
  BallotStuffing = 'BallotStuffing',
  ViolationsRelatedToControlPaper = 'ViolationsRelatedToControlPaper',
  NotCheckingVoterIdentificationSafeguardMeasures = 'NotCheckingVoterIdentificationSafeguardMeasures',
  VotingWithoutVoterIdentificationSafeguardMeasures = 'VotingWithoutVoterIdentificationSafeguardMeasures',
  BreachOfSecrecyOfVote = 'BreachOfSecrecyOfVote',
  ViolationsRelatedToMobileBallotBox = 'ViolationsRelatedToMobileBallotBox',
  NumberOfBallotsExceedsNumberOfVoters = 'NumberOfBallotsExceedsNumberOfVoters',
  ImproperInvalidationOrValidationOfBallots = 'ImproperInvalidationOrValidationOfBallots',
  FalsificationOrImproperCorrectionOfFinalProtocol = 'FalsificationOrImproperCorrectionOfFinalProtocol',
  RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy = 'RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy',
  ImproperFillingInOfFinalProtocol = 'ImproperFillingInOfFinalProtocol',
  ViolationOfSealingProceduresOfElectionMaterials = 'ViolationOfSealingProceduresOfElectionMaterials',
  ViolationsRelatedToVoterLists = 'ViolationsRelatedToVoterLists',
  Other = 'Other',
}


export type AddQuickReportAPIPayload = {
  id: string;
  electionRoundId: string;

  title: string;
  description: string;

  quickReportLocationType: QuickReportLocationType;
  incidentCategory: IncidentCategory;
  pollingStationId?: string;
  pollingStationDetails?: string;
};

export const addQuickReport = ({
  electionRoundId,
  ...payload
}: AddQuickReportAPIPayload): Promise<QuickReportsAPIResponse> => {
  return API.post(`election-rounds/${electionRoundId}/quick-reports`, payload).then(
    (res) => res.data,
  );
};
