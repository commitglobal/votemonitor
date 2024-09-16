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
    title: string;
    body: string;
    searchText?: string;
    level1Filter?: string;
    level2Filter?: string;
    level3Filter?: string;
    level4Filter?: string;
    level5Filter?: string;
    statusFilter?: string;
    tagsFilter?: string[];
  }