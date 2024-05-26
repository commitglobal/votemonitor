import { authApi } from '@/common/auth-api';
import { PageResponse } from '@/common/types';
import { useQuery, UseQueryResult } from '@tanstack/react-query';

import { ElectionEvent } from '../models/election-event';
import { ObserverGuide } from '../models/observer-guide';
import { electionRoundKeys } from '@/features/election-round/queries';

const STALE_TIME = 1000 * 60; // one minute

type ElectionEventResult = UseQueryResult<ElectionEvent, Error>;
export function useElectionRound(): ElectionEventResult {
  const electionRoundId = localStorage.getItem('electionRoundId') ?? '';

  return useQuery({
    queryKey: electionRoundKeys.detail(electionRoundId),
    queryFn: async () => {

      const response = await authApi.get<ElectionEvent>(`/election-rounds/${electionRoundId}`);

      return {
        ...response.data,
      };
    },
    staleTime: STALE_TIME,
  });
}
type ObserverGuideResult = UseQueryResult<PageResponse<ObserverGuide>, Error>;

export function useObserverGuides(): ObserverGuideResult {
  return useQuery({
    queryKey: ['observer-guides'],
    queryFn: async () => {
       const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<{guides:ObserverGuide[] }>(`/election-rounds/${electionRoundId}/observer-guide`);

      const pageResponse: PageResponse<ObserverGuide> = {
        currentPage: 1,
        pageSize: response.data.guides.length,
        items: response.data.guides,
        totalCount: response.data.guides.length
      };

      return pageResponse;
    },
    staleTime: STALE_TIME,
  });
}
