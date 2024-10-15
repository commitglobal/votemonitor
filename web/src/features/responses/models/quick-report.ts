import { QuickReportFollowUpStatus } from "@/common/types";
import { Attachment } from "./common";

export enum QuickReportLocationType {
  NotRelatedToAPollingStation = 'NotRelatedToAPollingStation',
  OtherPollingStation = 'OtherPollingStation',
  VisitedPollingStation = 'VisitedPollingStation',
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


export const IncidentCategoryList: IncidentCategory[] = [
  IncidentCategory.PhysicalViolenceIntimidationPressure,
  IncidentCategory.CampaigningAtPollingStation,
  IncidentCategory.RestrictionOfObserversRights,
  IncidentCategory.UnauthorizedPersonsAtPollingStation,
  IncidentCategory.ViolationDuringVoterVerificationProcess,
  IncidentCategory.VotingWithImproperDocumentation,
  IncidentCategory.IllegalRestrictionOfVotersRightToVote,
  IncidentCategory.DamagingOrSeizingElectionMaterials,
  IncidentCategory.ImproperFilingOrHandlingOfElectionDocumentation,
  IncidentCategory.BallotStuffing,
  IncidentCategory.ViolationsRelatedToControlPaper,
  IncidentCategory.NotCheckingVoterIdentificationSafeguardMeasures,
  IncidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures,
  IncidentCategory.BreachOfSecrecyOfVote,
  IncidentCategory.ViolationsRelatedToMobileBallotBox,
  IncidentCategory.NumberOfBallotsExceedsNumberOfVoters,
  IncidentCategory.ImproperInvalidationOrValidationOfBallots,
  IncidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol,
  IncidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy,
  IncidentCategory.ImproperFillingInOfFinalProtocol,
  IncidentCategory.ViolationOfSealingProceduresOfElectionMaterials,
  IncidentCategory.ViolationsRelatedToVoterLists,
  IncidentCategory.Other,
];
export interface QuickReport {
  id: string;
  address: string;
  description: string;
  email: string;
  observerName: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  number: string;
  numberOfAttachments: number;
  pollingStationId: string;
  pollingStationDetails: string;
  quickReportLocationType: QuickReportLocationType;
  timestamp: string;
  title: string;
  monitoringObserverId: string;
  attachments: Attachment[];
  followUpStatus: QuickReportFollowUpStatus;
  incidentCategory: IncidentCategory;
}

export interface TimestampsFilterOptions {
  firstSubmissionTimestamp: string;
  lastSubmissionTimestamp: string;
}

export interface QuickReportsFilters  {
  timestampsFilterOptions: TimestampsFilterOptions;
}