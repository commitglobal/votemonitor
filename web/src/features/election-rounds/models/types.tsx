import { ElectionRoundStatus } from '@/common/types';
import { NGOStatus } from '@/features/ngos/models/NGO';

export interface MonitoringNgoModel {
  id: string;
  name: string;
  ngoId: string;
  ngoStatus: NGOStatus;
}

export interface ElectionRoundModel {
  id: string;
  countryId: string;
  countryName: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: ElectionRoundStatus;
  createdOn: string;
  lastModifiedOn: string;
  monitoringNgos: MonitoringNgoModel[];
}
