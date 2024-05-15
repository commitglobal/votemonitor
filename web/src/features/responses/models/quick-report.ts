enum QuickReportLocationType {
  NotRelatedToAPollingStation = 'NotRelatedToAPollingStation',
  OtherPollingStation = 'OtherPollingStation',
  VisitedPollingStation = 'VisitedPollingStation ',
}

export interface Attachment {
  id: string;
  quickReportId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
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
  quickReportLocationType: QuickReportLocationType;
  timestamp: string;
  title: string;
  monitoringObserverId: string;
  attachments: Attachment[];
}
