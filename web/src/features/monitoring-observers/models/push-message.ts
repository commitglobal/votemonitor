import { FormSubmissionFollowUpStatus, FormType, QuestionsAnswered, QuickReportFollowUpStatus } from "@/common/types";
import { IncidentCategory } from "@/features/responses/models/quick-report";
import { MonitoringObserverStatus } from "./monitoring-observer";

export interface PushMessageModel {
    id: string;
    title: string;
    body: string;
    sender: string;
    sentAt: Date;
    numberOfTargetedObservers: number;
    numberOfReadNotifications: number;
}
export interface PushMessageReceiverModel{
    id: string;
    name: string;
    hasReadNotification: boolean;
}

export interface PushMessageDetailedModel {
    id: string;
    title: string;
    body: string;
    sender: string;
    sentAt: Date;
    receivers: PushMessageReceiverModel[];
}

export interface SendPushNotificationRequest {
  searchText?: string;
  statusFilter?: MonitoringObserverStatus;
  level1Filter?: string;
  level2Filter?: string;
  level3Filter?: string;
  level4Filter?: string;
  level5Filter?: string;
  pollingStationNumberFilter?: string;
  tagsFilter?: string[];
  submissionsFromDate?: string;
  submissionsToDate?: string;
  questionsAnswered?: QuestionsAnswered;
  formId?: string;
  formTypeFilter?: FormType;
  followUpStatus?: FormSubmissionFollowUpStatus;
  monitoringObserverId?: string;
  hasFlaggedAnswers?: boolean;
  hasNotes?: boolean;
  hasAttachments?: boolean;
  hasQuickReports?: boolean;
  monitoringObserverStatus?: MonitoringObserverStatus;
  quickReportIncidentCategory?: IncidentCategory;
  quickReportFollowUpStatus?: QuickReportFollowUpStatus;
  fromDateFilter?: string;
  toDateFilter?: string;
  isCompletedFilter?: boolean;
}