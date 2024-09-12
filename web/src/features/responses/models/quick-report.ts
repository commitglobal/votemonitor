import { FollowUpStatus } from '@/common/types';
import { Attachment } from './common';

export enum QuickReportLocationType {
  NotRelatedToAPollingStation = 'NotRelatedToAPollingStation',
  OtherPollingStation = 'OtherPollingStation',
  VisitedPollingStation = 'VisitedPollingStation',
}

export enum QuickReportOfficialComplaintFilingStatus {
  Yes = 'Yes',
  NoButPlanningToFile = 'NoButPlanningToFile',
  NoAndNotPlanningToFile = 'NoAndNotPlanningToFile',
  DoesNotApplyOrOther = 'DoesNotApplyOrOther',
}

export enum QuickReportIssueType {
  A = 'A',
  B = 'B',
  C = 'C',
  D = 'D',
}

export interface QuickReport {
  id: string;
  address: string;
  description: string;
  email: string;
  firstName: string;
  lastName: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  number: string;
  numberOfAttachments: number;
  phoneNumber: string;
  pollingStationId: string;
  pollingStationDetails: string;
  quickReportLocationType: QuickReportLocationType;
  issueType: QuickReportIssueType;
  officialComplaintFilingStatus: QuickReportOfficialComplaintFilingStatus;
  timestamp: string;
  title: string;
  monitoringObserverId: string;
  attachments: Attachment[];
  followUpStatus: FollowUpStatus;
}
