import API from '@/services/api';
import { queryOptions, useQuery, UseQueryResult } from '@tanstack/react-query';

import { GuideModel } from '../models/guide';
import { queryClient } from '@/main';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const observerGuidesKeys = {
  all: (electionRoundId: string) => ['observer-guides', electionRoundId] as const,
  details: (electionRoundId: string, id: string) =>
    [...observerGuidesKeys.all(electionRoundId), 'details', id] as const,
};

type ObserverGuideResult = UseQueryResult<GuideModel[], Error>;

export function useObserverGuides(electionRoundId: string): ObserverGuideResult {
  return useQuery({
    queryKey: observerGuidesKeys.all(electionRoundId),
    queryFn: async () => {
      const response = await API.get<{ guides: GuideModel[] }>(`/election-rounds/${electionRoundId}/observer-guide`);

      response.data.guides.forEach((guide) => {
        queryClient.setQueryData(observerGuidesKeys.details(electionRoundId, guide.id), guide);
      });

      return response.data.guides;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

export const observerGuideDetailsQueryOptions = (electionRoundId: string, guideId: string) => {
  return queryOptions({
    queryKey: observerGuidesKeys.details(electionRoundId, guideId),
    queryFn: async () => {
      const response = await API.get<GuideModel>(`/election-rounds/${electionRoundId}/observer-guide/${guideId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch observer guide details');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
};
