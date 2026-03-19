import { getAvailableMonitoringNgos } from '@/api/election-rounds/get-available-monitoring-ngos';
import { getMonitoringNgos } from '@/api/election-rounds/get-monitoring-ngos';
import { DataTableParameters, ElectionRoundStatus, PageResponse } from '@/common/types';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { MonitoringNgoModel } from '../../models/types';
const STALE_TIME = 1000 * 60 * 15; // fifteen minutes

export interface ElectionsRoundsQueryParams {
  searchText: string | undefined;
  countryId: string | undefined;
  electionRoundStatus: ElectionRoundStatus | undefined;
}

export const monitoringNgoKeys = {
  all: (electionRoundId: string) => ['monitoringNgos', electionRoundId] as const,
  availableForMonitoring: (electionRoundId: string, params: DataTableParameters) =>
    [...monitoringNgoKeys.all(electionRoundId), 'available', { ...params }] as const,
};

type MonitoringNgosPageResponse = {
  monitoringNgos: MonitoringNgoModel[];
};

export function useMonitoringNgos(electionRoundId: string): UseQueryResult<MonitoringNgosPageResponse, Error> {
  return useQuery({
    queryKey: monitoringNgoKeys.all(electionRoundId),
    placeholderData: { monitoringNgos: [] },
    queryFn: async () => {
      return getMonitoringNgos(electionRoundId);
    },
    enabled: !!electionRoundId,

    staleTime: STALE_TIME,
  });
}

export function useAvailableMonitoringNgos(
  electionRoundId: string,
  p: DataTableParameters
): UseQueryResult<PageResponse<MonitoringNgoModel>, Error> {
  return useQuery({
    queryKey: monitoringNgoKeys.availableForMonitoring(electionRoundId, p),
    queryFn: async () => {
      return getAvailableMonitoringNgos(electionRoundId, p);
    },
    staleTime: STALE_TIME,
  });
}
