import { ElectionRoundStatus } from '@/common/types';

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
}
