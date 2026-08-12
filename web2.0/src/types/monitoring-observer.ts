import { z } from 'zod'
import { SortOrder } from './common'

export enum MonitoringObserverStatus {
  Active = 'Active',
  Pending = 'Pending',
  Suspended = 'Suspended',
}

/** Fixed order, so the status filter never reshuffles between renders. */
export const MonitoringObserverStatusList: MonitoringObserverStatus[] = [
  MonitoringObserverStatus.Active,
  MonitoringObserverStatus.Pending,
  MonitoringObserverStatus.Suspended,
]

export interface MonitoringObserverModel {
  id: string
  firstName: string
  lastName: string
  displayName: string
  email: string
  status: MonitoringObserverStatus
  phoneNumber: string
  tags: string[]
  isOwnObserver: boolean
  /** Absent for observers who have never used the mobile app. */
  latestActivityAt?: string
}

/**
 * Everything the list is filtered, sorted and paged by lives here, which is
 * what keeps the whole view shareable and reloadable from its url alone.
 */
export const monitoringObserversSearchSchema = z.object({
  tags: z.array(z.string()).optional(),
  searchText: z.string().optional(),
  status: z.enum(MonitoringObserverStatus).optional(),
  sortColumnName: z.string().optional(),
  sortOrder: z.enum(SortOrder).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),
})

export type MonitoringObserversSearch = z.infer<
  typeof monitoringObserversSearchSchema
>
