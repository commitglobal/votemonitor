import { FollowUpStatus } from "@/common/types";
import { Attachment } from "./common";

export enum QuickReportLocationType {
  NotRelatedToAPollingStation = 'NotRelatedToAPollingStation',
  OtherPollingStation = 'OtherPollingStation',
  VisitedPollingStation = 'VisitedPollingStation',
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
  timestamp: string;
  title: string;
  monitoringObserverId: string;
  attachments: Attachment[];
  followUpStatus: FollowUpStatus;
}
