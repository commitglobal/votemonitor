import API from "@/services/api";

export const listMonitoringObserversTags = (
  electionRoundId: string
): Promise<string[]> => {
  return API.get(
    `election-rounds/${electionRoundId}/monitoring-observers:tags`
  ).then((res) => res.data.tags);
};
