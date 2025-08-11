import { getCoalitionDetails } from "@/services/api/coalitions/get-my.api";
import { useQuery } from "@tanstack/react-query";

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const coalitionKeys = {
  my: (electionRoundId: string) =>
    ["coalitions", "my", electionRoundId] as const,
};

export const useCoalitionDetails = (electionRoundId: string) => {
  return useQuery({
    queryKey: coalitionKeys.my(electionRoundId),
    queryFn: async () => await getCoalitionDetails(electionRoundId),
    staleTime: STALE_TIME,
  });
};
