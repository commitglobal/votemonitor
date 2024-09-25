import { authApi } from '@/common/auth-api';
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
      const response = await authApi.get<{ guides: GuideModel[] }>(
        `/election-rounds/${electionRoundId}/citizen-guides`
      );

      response.data.guides.forEach((guide) => {
        queryClient.setQueryData(citizenGuidesKeys.details(electionRoundId, guide.id), guide);
      });

      return response.data.guides;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

export const citizenGuideDetailsQueryOptions = (electionRoundId: string, guideId: string) => {
  return queryOptions({
    queryKey: citizenGuidesKeys.details(electionRoundId, guideId),
    queryFn: async () => {
      const response = await authApi.get<GuideModel>(`/election-rounds/${electionRoundId}/citizen-guides/${guideId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch citizen guide details');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
};
