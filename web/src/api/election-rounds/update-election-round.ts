import { authApi } from '@/common/auth-api';
import type { ElectionRoundRequest } from '@/features/election-rounds/components/ElectionRoundForm/ElectionRoundForm';
import { DateOnlyFormat } from '@/common/formats';
import { format } from 'date-fns/format';

export function updateElectionRound(electionRoundId: string, electionRound: ElectionRoundRequest) {
  return authApi.put(`/election-rounds/${electionRoundId}`, {
    ...electionRound,
    startDate: format(electionRound.startDate, DateOnlyFormat),
  });
}

