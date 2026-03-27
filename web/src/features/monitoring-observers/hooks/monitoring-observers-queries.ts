import { getMonitoringObservers } from '@/api/monitoring-observers/get-monitoring-observers';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import { MonitoringObserver } from '../models/monitoring-observer';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const monitoringObserversKeys = {
  all: (electionRoundId: string) => ['monitoring-observers', electionRoundId] as const,
  lists: (electionRoundId: string) => [...monitoringObserversKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...monitoringObserversKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...monitoringObserversKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...monitoringObserversKeys.details(electionRoundId), id] as const,
  tags: (electionRoundId: string) => [...monitoringObserversKeys.details(electionRoundId), 'tags'] as const,
};

type MonitoringObserverResponse = PageResponse<MonitoringObserver>;

type UseMonitoringObserversResult = UseQueryResult<MonitoringObserverResponse, Error>;

export const useMonitoringObservers = (
  electionRoundId: string,
  queryParams: DataTableParameters
): UseMonitoringObserversResult => {
  return useQuery({
    queryKey: monitoringObserversKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      return getMonitoringObservers(electionRoundId, queryParams);
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME
  });
};
