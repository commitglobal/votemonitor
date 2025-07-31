import type { ElectionStatus } from "./election";

export interface MonitoredElection {
  id: string;
  countryId: string;
  countryIso2: string;
  countryIso3: string;
  countryNumericCode: string;
  countryName: string;
  countryFullName: string;
  title: string;
  englishTitle: string;
  startDate: string; // ISO 8601 date string
  status: ElectionStatus;
  createdOn: string; // ISO 8601 timestamp
  lastModifiedOn: string; // ISO 8601 timestamp
  isMonitoringNgoForCitizenReporting: boolean;
  isCoalitionLeader: boolean;
  coalitionId: string;
  coalitionName: string;
  numberOfNgosMonitoring: number | null;
  monitoringNgos: any[] | null;
}
