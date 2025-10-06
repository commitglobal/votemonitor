import { z } from "zod";
import { DataSource, SortOrder, type AttachmentModel } from "./common";

export enum QuickReportFollowUpStatus {
  NotApplicable = "NotApplicable",
  NeedsFollowUp = "NeedsFollowUp",
  Resolved = "Resolved",
}

export const QuickReportFollowUpStatusList: QuickReportFollowUpStatus[] = [
  QuickReportFollowUpStatus.NotApplicable,
  QuickReportFollowUpStatus.NeedsFollowUp,
  QuickReportFollowUpStatus.Resolved,
];

export enum QuickReportLocationType {
  NotRelatedToAPollingStation = "NotRelatedToAPollingStation",
  OtherPollingStation = "OtherPollingStation",
  VisitedPollingStation = "VisitedPollingStation",
}

export const QuickReportLocationTypeList: QuickReportLocationType[] = [
  QuickReportLocationType.NotRelatedToAPollingStation,
  QuickReportLocationType.VisitedPollingStation,
  QuickReportLocationType.OtherPollingStation,
];

export enum QuickReportIncidentCategory {
  PhysicalViolenceIntimidationPressure = "PhysicalViolenceIntimidationPressure",
  CampaigningAtPollingStation = "CampaigningAtPollingStation",
  RestrictionOfObserversRights = "RestrictionOfObserversRights",
  UnauthorizedPersonsAtPollingStation = "UnauthorizedPersonsAtPollingStation",
  ViolationDuringVoterVerificationProcess = "ViolationDuringVoterVerificationProcess",
  VotingWithImproperDocumentation = "VotingWithImproperDocumentation",
  IllegalRestrictionOfVotersRightToVote = "IllegalRestrictionOfVotersRightToVote",
  DamagingOrSeizingElectionMaterials = "DamagingOrSeizingElectionMaterials",
  ImproperFilingOrHandlingOfElectionDocumentation = "ImproperFilingOrHandlingOfElectionDocumentation",
  BallotStuffing = "BallotStuffing",
  ViolationsRelatedToControlPaper = "ViolationsRelatedToControlPaper",
  NotCheckingVoterIdentificationSafeguardMeasures = "NotCheckingVoterIdentificationSafeguardMeasures",
  VotingWithoutVoterIdentificationSafeguardMeasures = "VotingWithoutVoterIdentificationSafeguardMeasures",
  BreachOfSecrecyOfVote = "BreachOfSecrecyOfVote",
  ViolationsRelatedToMobileBallotBox = "ViolationsRelatedToMobileBallotBox",
  NumberOfBallotsExceedsNumberOfVoters = "NumberOfBallotsExceedsNumberOfVoters",
  ImproperInvalidationOrValidationOfBallots = "ImproperInvalidationOrValidationOfBallots",
  FalsificationOrImproperCorrectionOfFinalProtocol = "FalsificationOrImproperCorrectionOfFinalProtocol",
  RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy = "RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy",
  ImproperFillingInOfFinalProtocol = "ImproperFillingInOfFinalProtocol",
  ViolationOfSealingProceduresOfElectionMaterials = "ViolationOfSealingProceduresOfElectionMaterials",
  ViolationsRelatedToVoterLists = "ViolationsRelatedToVoterLists",
  Other = "Other",
}

export const QuickReportIncidentCategoryList: QuickReportIncidentCategory[] = [
  QuickReportIncidentCategory.PhysicalViolenceIntimidationPressure,
  QuickReportIncidentCategory.CampaigningAtPollingStation,
  QuickReportIncidentCategory.RestrictionOfObserversRights,
  QuickReportIncidentCategory.UnauthorizedPersonsAtPollingStation,
  QuickReportIncidentCategory.ViolationDuringVoterVerificationProcess,
  QuickReportIncidentCategory.VotingWithImproperDocumentation,
  QuickReportIncidentCategory.IllegalRestrictionOfVotersRightToVote,
  QuickReportIncidentCategory.DamagingOrSeizingElectionMaterials,
  QuickReportIncidentCategory.ImproperFilingOrHandlingOfElectionDocumentation,
  QuickReportIncidentCategory.BallotStuffing,
  QuickReportIncidentCategory.ViolationsRelatedToControlPaper,
  QuickReportIncidentCategory.NotCheckingVoterIdentificationSafeguardMeasures,
  QuickReportIncidentCategory.VotingWithoutVoterIdentificationSafeguardMeasures,
  QuickReportIncidentCategory.BreachOfSecrecyOfVote,
  QuickReportIncidentCategory.ViolationsRelatedToMobileBallotBox,
  QuickReportIncidentCategory.NumberOfBallotsExceedsNumberOfVoters,
  QuickReportIncidentCategory.ImproperInvalidationOrValidationOfBallots,
  QuickReportIncidentCategory.FalsificationOrImproperCorrectionOfFinalProtocol,
  QuickReportIncidentCategory.RefusalToIssueCopyOfFinalProtocolOrIssuingImproperCopy,
  QuickReportIncidentCategory.ImproperFillingInOfFinalProtocol,
  QuickReportIncidentCategory.ViolationOfSealingProceduresOfElectionMaterials,
  QuickReportIncidentCategory.ViolationsRelatedToVoterLists,
  QuickReportIncidentCategory.Other,
];

export const quickReportsSearchSchema = z.object({
  dataSource: z.enum(DataSource).default(DataSource.Ngo).catch(DataSource.Ngo),
  searchText: z.string().optional().default(""),
  sortColumnName: z.string().optional(),
  sortOrder: z.enum(SortOrder).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),

  level1Filter: z.string().optional(),
  level2Filter: z.string().optional(),
  level3Filter: z.string().optional(),
  level4Filter: z.string().optional(),
  level5Filter: z.string().optional(),
  pollingStationNumberFilter: z.string().optional(),
  quickReportFollowUpStatus: z.enum(QuickReportFollowUpStatus).optional(),
  quickReportLocationType: z.enum(QuickReportLocationType).optional(),
  incidentCategory: z.enum(QuickReportIncidentCategory).optional(),
  coalitionMemberId: z.string().optional(),
  pollingStationId: z.string().optional(),
  observerId: z.string().optional(),
  // TODO: add filter by has attachments
  // TODO: add filter by dates
});

export type QuickReportsSearch = z.infer<typeof quickReportsSearchSchema>;

export interface QuickReportModel {
  id: string;
  address: string;
  description: string;
  phoneNumber: string;
  email: string;
  ngoName: string;
  observerName: string;
  isOwnObserver: boolean;
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
  attachments: AttachmentModel[];
  followUpStatus: QuickReportFollowUpStatus;
  incidentCategory: QuickReportIncidentCategory;
}
