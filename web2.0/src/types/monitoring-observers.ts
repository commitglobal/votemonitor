import { z } from "zod";
import { SortOrder } from "./common";

export enum MonitoringObserverStatus {
  Active = "Active",
  Pending = "Pending",
  Suspended = "Suspended",
}

export interface MonitoringObserver {
  id: string;
  firstName: string;
  lastName: string;
  displayName: string;
  email: string;
  status: MonitoringObserverStatus;
  phoneNumber: string;
  tags: string[];
  isOwnObserver: boolean;
  latestActivityAt?: string;
}

export const monitoringObserversSearchSchema = z.object({
  tags: z.array(z.string()).optional(),
  searchText: z.string().optional(),
  status: z.nativeEnum(MonitoringObserverStatus).optional(),
  sortColumnName: z.string().optional(),
  sortOrder: z.nativeEnum(SortOrder).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),
});

export type MonitoringObserversSearch = z.infer<
  typeof monitoringObserversSearchSchema
>;
