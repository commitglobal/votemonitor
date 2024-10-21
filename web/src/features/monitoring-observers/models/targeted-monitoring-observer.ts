import { MonitoringObserverStatus } from './monitoring-observer';

export interface TargetedMonitoringObserver {
  id: string;
  observerName: string;
  email: string;
  status: MonitoringObserverStatus;
  phoneNumber: string;
  tags: string[];
  numberOfFormsSubmitted: number;
  numberOfLocations: number;
  latestActivityAt?: string;
}
