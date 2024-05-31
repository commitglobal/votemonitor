import { FollowUpStatus } from "@/common/types";
import { QuickReportLocationType } from "../models/quick-report";

export function mapQuickReportLocationType(locationType: QuickReportLocationType): string {
    if (locationType === QuickReportLocationType.NotRelatedToAPollingStation) return 'Not Related To A Polling Station';
    if (locationType === QuickReportLocationType.OtherPollingStation) return 'Other Polling Station';
    if (locationType === QuickReportLocationType.VisitedPollingStation) return 'Visited Polling Station';
  
    return 'Unknown';
  };


export function mapFollowUpStatus(followUpStatus: FollowUpStatus): string {
    if (followUpStatus === FollowUpStatus.NotApplicable) return 'Not Applicable';
    if (followUpStatus === FollowUpStatus.NeedsFollowUp) return 'Needs Follow-up';
    if (followUpStatus === FollowUpStatus.Resolved) return 'Resolved';
  
    return 'Unknown';
  };