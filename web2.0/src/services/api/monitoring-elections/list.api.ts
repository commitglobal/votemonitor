import API from "@/services/api";
import type { MonitoredElection } from "@/types/monitored-election";

export const listMonitoredElections = (): Promise<MonitoredElection[]> => {
  return API.get(`/election-rounds:monitoring`).then(
    (res) => res.data.electionRounds
  );
};
