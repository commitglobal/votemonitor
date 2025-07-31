import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { ElectionsSearch } from "@/types/election";
import type { MonitoredElection } from "@/types/monitored-election";

export const listMonitoredElections = (
  search: ElectionsSearch
): Promise<MonitoredElection[]> => {
  return API.get(`/election-rounds:monitoring`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data.electionRounds);
};
