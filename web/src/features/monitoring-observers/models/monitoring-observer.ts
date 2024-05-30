/* eslint-disable unicorn/prefer-top-level-await */
import { FollowUpStatus } from '@/common/types';
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

export const monitoringObserverRouteSearchSchema = z.object({
  searchText: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  status: z.enum(['Active', 'Inactive', 'Suspended']).catch('Active'),
});

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
  followUpStatus: z.enum([FollowUpStatus.NotApplicable, FollowUpStatus.NeedsFollowUp, FollowUpStatus.Resolved]).optional()
});

export type MonitoringObserverDetailsRouteSearch = z.infer<typeof monitoringObserverDetailsRouteSearchSchema>;


export interface UpdateMonitoringObserverRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  status: string;
  tags: string[];
}