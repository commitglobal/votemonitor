import { getObserverGuideDetails } from '@/api/election-event/get-observer-guide-details';
import { getObserverGuides } from '@/api/election-event/get-observer-guides';
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
      const response = await getObserverGuides(electionRoundId);

      response.guides.forEach((guide) => {
        queryClient.setQueryData(observerGuidesKeys.details(electionRoundId, guide.id), guide);
      });

      return response.guides;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

export const observerGuideDetailsQueryOptions = (electionRoundId: string, guideId: string) => {
  return queryOptions({
    queryKey: observerGuidesKeys.details(electionRoundId, guideId),
    queryFn: async () => {
      return getObserverGuideDetails(electionRoundId, guideId);
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
};
