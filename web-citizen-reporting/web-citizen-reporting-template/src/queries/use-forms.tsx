import type { ElectionModel } from "@/common/types";
import { useQuery } from "@tanstack/react-query";

export const useElections = <TResult = Array<ElectionModel>,>(
  select?: (elections: Array<ElectionModel>) => TResult
) => {
  return useQuery({
    queryKey: ["guides"],
    placeholderData: [],
    queryFn: async () => {
      const response = await fetch(`/elections.json`);
      return await response.json();
    },
    select,
  });
};

export function useElectionById(electionId: string) {
  return useQuery({
    queryKey: ["guides", electionId],
    queryFn: async (): Promise<ElectionModel> => {
      const response = await fetch(`/elections-data/${electionId}.json`);
      return await response.json();
    },
  });
}
