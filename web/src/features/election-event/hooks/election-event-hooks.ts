import { authApi } from '@/common/auth-api';
import { PageResponse } from '@/common/types';
import { useQuery, UseQueryResult } from '@tanstack/react-query';

import { electionRoundKeys } from '@/features/election-round/queries';
import { ElectionEvent } from '../models/election-event';
import { ObserverGuide } from '../models/observer-guide';

const STALE_TIME = 1000 * 60 * 5; // five minutes

type ElectionEventResult = UseQueryResult<ElectionEvent, Error>;
export function useElectionRoundDetails(electionRoundId: string): ElectionEventResult {
  return useQuery({
    queryKey: electionRoundKeys.detail(electionRoundId!),
    queryFn: async () => {

      const response = await authApi.get<ElectionEvent>(`/election-rounds/${electionRoundId}`);

      return {
        ...response.data,
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}
type ObserverGuideResult = UseQueryResult<PageResponse<ObserverGuide>, Error>;

export function useObserverGuides(electionRoundId: string): ObserverGuideResult {
  return useQuery({
    queryKey: ['observer-guides'],
    queryFn: async () => {

      const response = await authApi.get<{ guides: ObserverGuide[] }>(`/election-rounds/${electionRoundId}/observer-guide`);

      const pageResponse: PageResponse<ObserverGuide> = {
        currentPage: 1,
        pageSize: response.data.guides.length,
        items: response.data.guides,
        totalCount: response.data.guides.length
      };

      return pageResponse;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}
