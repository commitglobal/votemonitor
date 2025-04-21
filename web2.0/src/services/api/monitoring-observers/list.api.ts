import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type {
  MonitoringObserver,
  MonitoringObserversSearch,
} from "@/types/monitoring-observers";

export const listMonitoringObservers = (
  electionRoundId: string,
  search: MonitoringObserversSearch
): Promise<PageResponse<MonitoringObserver>> => {
  return API.get(`election-rounds/${electionRoundId}/monitoring-observers`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data);
};
