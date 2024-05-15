/* eslint-disable unicorn/prefer-top-level-await */
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
}

export const monitoringObserverRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  status: z.enum(['Active', 'Inactive', 'Suspended']).catch('Active'),
});
