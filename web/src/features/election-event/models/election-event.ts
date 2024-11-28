import { ElectionRoundStatus } from '@/common/types';

export interface ElectionEvent {
  id: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: ElectionRoundStatus;
  countryId: string;
  countryIso2: string;
  countryIso3: string;
  countryNumericCode: string;
  countryName: string;
  countryFullName: string;
  createdOn: string;
  lastModifiedOn: string;
  monitoringNgoId: string;
  country: string;
  isMonitoringNgoForCitizenReporting: boolean;
  isCoalitionLeader: boolean;
}
