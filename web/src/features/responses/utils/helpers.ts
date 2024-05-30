import { QuickReportLocationType } from "../models/quick-report";

export function mapQuickReportLocationType(locationType: QuickReportLocationType): string {
    if (locationType === QuickReportLocationType.NotRelatedToAPollingStation) return 'Not Related To A Polling Station';
    if (locationType === QuickReportLocationType.OtherPollingStation) return 'Other Polling Station';
    if (locationType === QuickReportLocationType.VisitedPollingStation) return 'Visited Polling Station';
  
    return 'Unknown';
  };