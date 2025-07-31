import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type {
  MonitoringObserverModel,
  MonitoringObserversSearch,
} from "@/types/monitoring-observer";

export const listMonitoringObservers = (
  electionRoundId: string,
  search: MonitoringObserversSearch
): Promise<PageResponse<MonitoringObserverModel>> => {
  return API.get(`election-rounds/${electionRoundId}/monitoring-observers`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data);
};
