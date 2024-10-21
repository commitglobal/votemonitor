/* eslint-disable unicorn/prefer-top-level-await */
import { FormSubmissionFollowUpStatus, IncidentReportFollowUpStatus, QuickReportFollowUpStatus } from '@/common/types';
import { IncidentReportLocationType } from '@/features/responses/models/incident-report';
import { QuickReportLocationType } from '@/features/responses/models/quick-report';
import { z } from 'zod';

export enum MonitoringObserverStatus {
  Active = 'Active',
  Pending = 'Pending',
  Suspended = 'Suspended',
}

export interface MonitoringObserver {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  status: MonitoringObserverStatus;
  phoneNumber: string;
  tags: string[];
  latestActivityAt?: string;
}

export const monitoringObserverDetailsRouteSearchSchema = z.object({
  formCodeFilter: z.string().catch('').optional(),
  formTypeFilter: z.string().catch('').optional(),
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  hasFlaggedAnswers: z.string().catch('').optional(),
  monitoringObserverId: z.string().catch('').optional(),
  followUpStatus: z.nativeEnum(FormSubmissionFollowUpStatus).optional(),

  quickReportFollowUpStatus: z.nativeEnum(QuickReportFollowUpStatus).optional(),
  quickReportLocationType: z.nativeEnum(QuickReportLocationType).optional(),

  incidentReportFollowUpStatus: z.nativeEnum(IncidentReportFollowUpStatus).optional(),
  incidentReportLocationType: z.nativeEnum(IncidentReportLocationType).optional(),
});

export type MonitoringObserverDetailsRouteSearch = z.infer<typeof monitoringObserverDetailsRouteSearchSchema>;

export interface UpdateMonitoringObserverRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  status: string;
  tags: string[];
}
