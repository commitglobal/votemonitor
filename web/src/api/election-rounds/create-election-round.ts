import { authApi } from '@/common/auth-api';
import type { ElectionRoundModel } from '@/features/election-rounds/models/types';
import type { ElectionRoundRequest } from '@/features/election-rounds/components/ElectionRoundForm/ElectionRoundForm';
import { DateOnlyFormat } from '@/common/formats';
import { format } from 'date-fns/format';

export function createElectionRound(electionRound: ElectionRoundRequest) {
  return authApi.post<ElectionRoundModel>(`/election-rounds`, {
    ...electionRound,
    startDate: format(electionRound.startDate, DateOnlyFormat),
  });
}

