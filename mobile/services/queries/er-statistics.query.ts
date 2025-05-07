import { skipToken, useQuery } from "@tanstack/react-query";
import { getERStatistics } from "../api/get-er-statistics.api";
export const ERStatisticsKeys = {
  all: ["er-statistics"] as const,
  byElectionRound: (electionRoundId: string | undefined) => [
    ...ERStatisticsKeys.all,
    "electionRoundId",
    electionRoundId,
  ],
};

export const useElectionRoundStatistics = (electionRoundId?: string) => {
  return useQuery({
    queryKey: ERStatisticsKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId ? () => getERStatistics(electionRoundId) : skipToken,
  });
};
