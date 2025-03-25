import { authApi } from '@/common/auth-api';
import {
  useMutation,
  type UseMutationOptions,
  type UseMutationResult,
  useQuery,
  type UseQueryOptions,
  type UseQueryResult,
} from '@tanstack/react-query';
import { type DataExport, type ExportedDataDetails, ExportedDataType, ExportStatus } from '../models/data-export';

type UseDataExportOptions = UseMutationOptions<DataExport, Error, void>;

export function useStartDataExport(
  {
    electionRoundId,
    exportedDataType,
    filterParams,
  }: {
    electionRoundId: string;
    exportedDataType: ExportedDataType;
    filterParams?: Record<string, any>;
  },
  options?: UseDataExportOptions
): UseMutationResult<DataExport, Error, void> {
  return useMutation({
    mutationFn: async () => {
      const response = await authApi.post<DataExport>(`/exported-data`, {
        electionRoundId,
        exportedDataType,
        formSubmissionsFilters: exportedDataType === ExportedDataType.FormSubmissions ? filterParams : undefined,
        quickReportsFilters: exportedDataType === ExportedDataType.QuickReports ? filterParams : undefined,
        citizenReportsFilters: exportedDataType === ExportedDataType.CitizenReports ? filterParams : undefined,
        incidentReportsFilters: exportedDataType === ExportedDataType.IncidentReports ? filterParams : undefined,
      });

      return response.data;
    },
    ...options,
  });
}

type UseExportedDataDetailsOptions = Omit<UseQueryOptions<ExportedDataDetails>, 'queryKey'>;

export function useExportedDataDetails(
  {
    electionRoundId,
    exportedDataId,
  }: {
    electionRoundId: string;
    exportedDataId: string;
  },
  options?: UseExportedDataDetailsOptions
): UseQueryResult<ExportedDataDetails> {
  return useQuery({
    queryKey: ['exported-data-details', exportedDataId],
    queryFn: async () => {
      const response = await authApi.get<ExportedDataDetails>(`/exported-data/${exportedDataId}:details`);

      return response.data;
    },
    enabled: !!electionRoundId && options?.enabled && Boolean(exportedDataId),
    refetchInterval: ({ state }) => (state.data?.exportStatus === ExportStatus.Started ? 5 * 1000 : undefined),
  });
}
