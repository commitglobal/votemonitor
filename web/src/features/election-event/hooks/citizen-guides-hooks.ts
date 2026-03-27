import { getCitizenGuideDetails } from '@/api/election-event/get-citizen-guide-details';
import { getCitizenGuides } from '@/api/election-event/get-citizen-guides';
import { queryOptions, useQuery, UseQueryResult } from '@tanstack/react-query';

import { queryClient } from '@/main';
import { GuideModel } from '../models/guide';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const citizenGuidesKeys = {
  all: (electionRoundId: string) => ['citizen-guides', electionRoundId] as const,
  details: (electionRoundId: string, id: string) => [...citizenGuidesKeys.all(electionRoundId), 'details', id] as const,
};

type CitizenGuideResult = UseQueryResult<GuideModel[], Error>;

export function useCitizenGuides(electionRoundId: string): CitizenGuideResult {
  return useQuery({
    queryKey: citizenGuidesKeys.all(electionRoundId),
    queryFn: async () => {
      const response = await getCitizenGuides(electionRoundId);

      response.guides.forEach((guide) => {
        queryClient.setQueryData(citizenGuidesKeys.details(electionRoundId, guide.id), guide);
      });

      return response.guides;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

export const citizenGuideDetailsQueryOptions = (electionRoundId: string, guideId: string) => {
  return queryOptions({
    queryKey: citizenGuidesKeys.details(electionRoundId, guideId),
    queryFn: async () => {
      return getCitizenGuideDetails(electionRoundId, guideId);
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
};
